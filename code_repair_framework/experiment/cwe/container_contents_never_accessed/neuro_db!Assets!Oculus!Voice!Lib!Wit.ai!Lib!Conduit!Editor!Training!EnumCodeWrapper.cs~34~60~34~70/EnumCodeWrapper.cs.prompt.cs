/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Meta.WitAi;
using Microsoft.CSharp;

namespace Meta.Conduit.Editor
{
    /// <summary>
    /// Wraps around an Enum in code to allow querying and modifying its source code in a single source file.
    /// </summary>
    internal class EnumCodeWrapper
    {
        public const string DEFAULT_PATH = @"Assets\";
        
        private readonly string _sourceFilePath;
        private readonly IFileIo _fileIo;
        private readonly CodeCompileUnit _compileUnit;
        private readonly CodeTypeDeclaration _typeDeclaration;
        private readonly List<string> _enumValues = new List<string>();
        private readonly CodeDomProvider _provider = new CSharpCodeProvider();
        private readonly Dictionary<string, CodeNamespace> _namespaces = new Dictionary<string, CodeNamespace>();
        private readonly Action<CodeNamespace> _namespaceSetup;
        private readonly Action<CodeMemberField> _memberSetup;
        private readonly string _conduitAttributeName;
        private readonly CodeNamespace _namespace;

        // Setup with existing enum
        public EnumCodeWrapper(IFileIo fileIo, Type enumType, string entityName, string sourceCodeFile) : this(fileIo, enumType.Name, entityName, null, enumType.Namespace, sourceCodeFile)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type must be an enumeration.", nameof(enumType));
            }

            var enumValues = new List<WitKeyword>();
            foreach (var enumValueName in enumType.GetEnumNames())
            {
                var aliases = GetAliases(enumType, enumValueName);
                enumValues.Add(new WitKeyword(aliases[0], aliases.GetRange(1, aliases.Count - 1)));
            }
            
            AddValues(enumValues);
        }

        // Setup
        public EnumCodeWrapper(IFileIo fileIo, string enumName, string entityName, IList<WitKeyword> enumValues, string enumNamespace = null, string sourceCodeFile = null)
        {
        // BUG: Container contents are never accessed
        // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        //         private readonly Dictionary<string, CodeNamespace> _namespaces = new Dictionary<string, CodeNamespace>();
        //         private readonly Action<CodeNamespace> _namespaceSetup;
        //         private readonly Action<CodeMemberField> _memberSetup;
        //         private readonly string _conduitAttributeName;
        //         private readonly CodeNamespace _namespace;
        // 
        //         // Setup with existing enum
        //         public EnumCodeWrapper(IFileIo fileIo, Type enumType, string entityName, string sourceCodeFile) : this(fileIo, enumType.Name, entityName, null, enumType.Namespace, sourceCodeFile)
        //         {
        //             if (!enumType.IsEnum)
        //             {
        //                 throw new ArgumentException("Type must be an enumeration.", nameof(enumType));
        //             }
        // 
        //             var enumValues = new List<WitKeyword>();
        //             foreach (var enumValueName in enumType.GetEnumNames())
        //             {
        //                 var aliases = GetAliases(enumType, enumValueName);
        //                 enumValues.Add(new WitKeyword(aliases[0], aliases.GetRange(1, aliases.Count - 1)));
        //             }
        //             
        //             AddValues(enumValues);
        //         }
        // 
        //         // Setup
        //         public EnumCodeWrapper(IFileIo fileIo, string enumName, string entityName, IList<WitKeyword> enumValues, string enumNamespace = null, string sourceCodeFile = null)
        //         {
        //             if (string.IsNullOrEmpty(enumName))
        //             {
        //                 throw new ArgumentException(nameof(enumName));
        //             }
        //             if (string.IsNullOrEmpty(entityName))
        //             {
        //                 throw new ArgumentException(nameof(entityName));
        //             }
        //             
        //             _conduitAttributeName = GetShortAttributeName(nameof(ConduitValueAttribute));
        //             
        //             // Initial setup
        //             _compileUnit = new CodeCompileUnit();
        //             _sourceFilePath = string.IsNullOrEmpty(sourceCodeFile) ? GetEnumFilePath(enumName, enumNamespace) : sourceCodeFile;
        //             _fileIo = fileIo;
        // 
        //             // Setup namespace
        //             if (string.IsNullOrEmpty(enumNamespace))
        //             {
        //                 _namespace = new CodeNamespace();
        //             }
        //             else
        //             {
        //                 _namespace = new CodeNamespace(enumNamespace);
        //                 _namespaces.Add(enumNamespace, _namespace);
        //             }
        // 
        //             _compileUnit.Namespaces.Add(_namespace);
        // 
        //             // Setup type declaration
        //             _typeDeclaration = new CodeTypeDeclaration(enumName)
        //             {
        //                 IsEnum = true
        //             };
        //             _namespace.Types.Add(_typeDeclaration);
        // 
        //             if (!entityName.Equals(enumName))
        //             {
        //                 var entityAttributeType = new CodeTypeReference(GetShortAttributeName(nameof(ConduitEntityAttribute)));
        //                 var entityAttributeArgs = new CodeAttributeArgument[]
        //                 {
        //                     new CodeAttributeArgument(new CodePrimitiveExpression(entityName))
        //                 };
        //                 AddEnumAttribute(new CodeAttributeDeclaration(entityAttributeType, entityAttributeArgs));
        //             }
        // 
        //             // Add all enum values
        //             AddValues(enumValues);
        //         }

        // FIXED VERSION:
