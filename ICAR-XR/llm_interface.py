"""
LLM Interface Module

Provides unified interface for interacting with different LLM models.
Integrates with existing XRFix LLM infrastructure.
"""

from typing import Dict, Optional, Any
from abc import ABC, abstractmethod
import logging


class LLMInterface(ABC):
    """Abstract base class for LLM interfaces"""
    
    @abstractmethod
    def generate(self, prompt: str, model: str = None, 
                temperature: float = 0.7, max_tokens: int = 2000,
                **kwargs) -> Dict[str, Any]:
        """
        Generate response from LLM
        
        Args:
            prompt: Input prompt
            model: Model name
            temperature: Sampling temperature
            max_tokens: Maximum tokens to generate
            
        Returns:
            Dictionary with 'content' key and optional metadata
        """
        pass


class GPTLLMInterface(LLMInterface):
    """
    Interface for GPT models (integrates with existing XRFix GPT support)
    """
    
    def __init__(self, api_key: str = None, base_url: str = None):
        """Initialize GPT interface"""
        self.api_key = api_key
        self.base_url = base_url
        self.logger = logging.getLogger('ICAR-XR.LLM')
        
        # Try to use existing XRFix GPT infrastructure
        try:
            import sys
            import os
            sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
            from experiment_gen_response_GPT import generate_gpt4_requests_no_max_tokens
            self.generate_func = generate_gpt4_requests_no_max_tokens
        except ImportError:
            self.generate_func = None
            self.logger.warning("Could not import XRFix GPT interface, using basic implementation")
    
    def generate(self, prompt: str, model: str = None,
                temperature: float = 0.7, max_tokens: int = 2000,
                **kwargs) -> Dict[str, Any]:
        """Generate response using GPT model"""
        
        if self.generate_func:
            try:
                response = self.generate_func(
                    instruct_head="",
                    prompt=prompt,
                    temp=temperature,
                    top_p=kwargs.get('top_p', 1.0),
                    iteration=1
                )
                return {
                    'content': response,
                    'model': model,
                    'confidence': 0.7
                }
            except Exception as e:
                self.logger.error(f"GPT generation failed: {e}")
        
        # Fallback: return placeholder
        return {
            'content': prompt,
            'model': model,
            'error': 'LLM generation not available'
        }


class OpenSourceLLMInterface(LLMInterface):
    """
    Interface for open-source models (StarCoder, CodeLlama, DeepSeek)
    Integrates with existing XRFix open-source infrastructure
    """
    
    def __init__(self, model_name: str = "starcoder", device: str = "cpu"):
        """Initialize open-source LLM interface"""
        self.model_name = model_name
        self.device = device
        self.logger = logging.getLogger('ICAR-XR.LLM')
        self.model = None
        self.tokenizer = None
        
        try:
            from transformers import AutoTokenizer, AutoModelForCausalLM
            self.logger.info(f"Loading {model_name}...")
            
            # Model paths
            model_paths = {
                'starcoder': 'bigcode/starcoder',
                'codellama': 'meta-llama/CodeLlama-7b-hf',
                'deepseek': 'deepseek-ai/deepseek-coder-6.7b-base'
            }
            
            model_path = model_paths.get(model_name, model_name)
            
            # Load tokenizer and model
            self.tokenizer = AutoTokenizer.from_pretrained(model_path)
            self.model = AutoModelForCausalLM.from_pretrained(
                model_path,
                device_map=device,
                torch_dtype="auto"
            )
            self.logger.info(f"Successfully loaded {model_name}")
        except Exception as e:
            self.logger.warning(f"Could not load {model_name}: {e}")
            self.logger.info("Will use mock responses instead")
    
    def generate(self, prompt: str, model: str = None,
                temperature: float = 0.7, max_tokens: int = 2000,
                **kwargs) -> Dict[str, Any]:
        """Generate response using open-source model"""
        
        if not self.model or not self.tokenizer:
            self.logger.warning(f"Model not loaded, returning mock response")
            return {
                'content': prompt,
                'model': self.model_name,
                'error': 'Model not loaded'
            }
        
        try:
            # Tokenize input
            inputs = self.tokenizer.encode(prompt, return_tensors="pt").to(self.device)
            
            # Generate
            outputs = self.model.generate(
                inputs,
                max_length=len(inputs[0]) + max_tokens,
                temperature=temperature,
                top_p=kwargs.get('top_p', 1.0),
                do_sample=True
            )
            
            # Decode output
            content = self.tokenizer.decode(outputs[0])
            
            return {
                'content': content,
                'model': self.model_name
            }
        except Exception as e:
            self.logger.error(f"Generation failed: {e}")
            return {
                'content': prompt,
                'model': self.model_name,
                'error': str(e)
            }


def create_llm_interface(model_name: str = "gpt-4",
                        api_key: str = None,
                        **kwargs) -> LLMInterface:
    """
    Factory function to create appropriate LLM interface
    
    Args:
        model_name: Name of the model to use
        api_key: API key if needed
        **kwargs: Additional arguments
        
    Returns:
        LLMInterface instance
    """
    
    return GPTLLMInterface(api_key=api_key)

