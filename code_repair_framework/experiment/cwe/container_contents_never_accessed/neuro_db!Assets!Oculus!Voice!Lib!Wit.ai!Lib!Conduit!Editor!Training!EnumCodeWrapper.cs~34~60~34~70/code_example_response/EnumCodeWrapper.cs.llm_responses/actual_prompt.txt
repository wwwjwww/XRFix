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
    
    
    
    internal class EnumCodeWrapper
    {
        public const string DEFAULT_PATH = @"Assets\";
        
        private readonly string _sourceFilePath;
        private readonly IFileIo _fileIo;
        private readonly CodeCompileUnit _compileUnit;
        private readonly CodeTypeDeclaration _typeDeclaration;
        private readonly List<string> _enumValues = new List<string>();
        private readonly CodeDomProvider _provider = new CSharpCodeProvider();
        // BUG: Container contents are never accessed
        // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        //         private readonly Dictionary<string, CodeNamespace> _namespaces = new Dictionary<string, CodeNamespace>();

        // FIXED CODE:
