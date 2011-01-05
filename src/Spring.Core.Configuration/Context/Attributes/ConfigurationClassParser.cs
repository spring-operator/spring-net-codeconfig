﻿#region License

/*
 * Copyright © 2002-2010 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Collections.Generic;
using Spring.Objects.Factory.Parsing;
using Spring.Collections.Generic;
using System.Reflection;

namespace Spring.Context.Attributes
{

    public class ConfigurationClassParser
    {
        private ISet<ConfigurationClass> _configurationClasses = new HashedSet<ConfigurationClass>();

        private Stack<ConfigurationClass> _importStack = new Stack<ConfigurationClass>();

        private IProblemReporter _problemReporter;

        /// <summary>
        /// Initializes a new instance of the ConfigurationClassParser class.
        /// </summary>
        /// <param name="problemReporter"></param>
        public ConfigurationClassParser(IProblemReporter problemReporter)
        {
            _problemReporter = problemReporter;
        }

        public ISet<ConfigurationClass> ConfigurationClasses
        {
            get { return _configurationClasses; }
        }

        //public void Parse(String className, String ObjectName)
        //{
        //    ProcessConfigurationClass(new ConfigurationClass(className, ObjectName));
        //}

        public void Parse(Type type, string objectName)
        {
            ProcessConfigurationClass(new ConfigurationClass(objectName, type));
        }

        public void Validate()
        {
            foreach (ConfigurationClass configClass in ConfigurationClasses)
            {
                configClass.Validate(_problemReporter);
            }
        }

        protected void ProcessConfigurationClass(ConfigurationClass configurationClass)
        {
            DoProcessConfigurationClass(configurationClass);

            if (ConfigurationClasses.Contains(configurationClass) && configurationClass.ObjectName != null)
            {
                // Explicit object definition found, probably replacing an import.
                // Let's remove the old one and go with the new one.
                ConfigurationClasses.Remove(configurationClass);
            }
            ConfigurationClasses.Add(configurationClass);
        }

        private void DoProcessConfigurationClass(ConfigurationClass configurationClass)
        {
            if (Attribute.GetCustomAttribute(configurationClass.ConfigurationClassType, typeof(ImportAttribute)) != null)
            {
                ImportAttribute attrib = Attribute.GetCustomAttribute(configurationClass.ConfigurationClassType, typeof(ImportAttribute)) as ImportAttribute;
                ProcessImport(configurationClass, attrib.Types);
            }

            if (Attribute.GetCustomAttribute(configurationClass.ConfigurationClassType, typeof(ImportResourceAttribute)) != null)
            {
                ImportResourceAttribute attrib = Attribute.GetCustomAttribute(configurationClass.ConfigurationClassType, typeof(ImportResourceAttribute)) as ImportResourceAttribute;

                foreach (string resource in attrib.Resources)
                {
                    configurationClass.AddImportedResource(resource, attrib.DefinitionReader);
                }
            }

            ISet<MethodInfo> definitionMethods = GetAllMethodsWithCustomAttributeForClass(configurationClass.ConfigurationClassType, typeof(DefinitionAttribute));
            foreach (MethodInfo definitionMethod in definitionMethods)
            {
                configurationClass.Methods.Add(new ConfigurationClassMethod(definitionMethod, configurationClass));

            }
        }

        public static ISet<MethodInfo> GetAllMethodsWithCustomAttributeForClass(Type theClass, Type customAttribute)
        {
            ISet<MethodInfo> methods = new HashedSet<MethodInfo>();

            foreach (MethodInfo method in theClass.GetMethods())
            {
                if (Attribute.GetCustomAttribute(method, customAttribute) != null)
                {
                    methods.Add(method);
                }
            }

            return methods;
        }

        private void ProcessImport(ConfigurationClass configClass, IEnumerable<Type> classesToImport)
        {
            if (_importStack.Contains(configClass))
            {
                _problemReporter.Error(new CircularImportProblem(configClass, _importStack, configClass.ConfigurationClassType));
            }
            else
            {
                _importStack.Push(configClass);
                foreach (Type classToImport in classesToImport)
                {
                    ProcessConfigurationClass(new ConfigurationClass(null, classToImport));
                }
                _importStack.Pop();
            }
        }

        private class CircularImportProblem : Problem
        {
            public CircularImportProblem(ConfigurationClass configClass, Stack<ConfigurationClass> importStack, Type configurationClassType)
                : base(String.Format("A circular [Import] has been detected: " +
                             "Illegal attempt by [Configuration] class '{0}' to import class '{1}' as '{2}' is " +
                             "already present in the current import stack [{3}]",
                             importStack.Peek().SimpleName, configClass.SimpleName,
                             configClass.SimpleName, importStack),
                      new Location(importStack.Peek().Resource, configurationClassType)
                )
            { }

        }

    }
}
