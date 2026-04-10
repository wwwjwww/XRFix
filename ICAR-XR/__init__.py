"""
ICAR-XR: Iterative Context-Augmented Repair Framework for XR Applications

A multi-stage, context-aware code repair framework that transforms one-shot
code repair into an iterative, feedback-driven "closed-loop reasoning" process.

Core Stages:
1. Initial Repair & Verification
2. XR-Specific Semantic Context Enhancement
3. Iterative Re-Repair
"""

__version__ = "1.0.0"
__author__ = "XRFix Team"

from .iterative_repair_engine import IterativeRepairEngine
from .context_extractor import ContextExtractor
from .xr_semantic_analyzer import XRSemanticAnalyzer

__all__ = [
    'IterativeRepairEngine',
    'ContextExtractor',
    'XRSemanticAnalyzer'
]
