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
using System.Linq;
using System.Reflection;
using System.Text;
using Spring.Context.Attributes;
using Spring.Objects.Factory.Support;

namespace Spring.Context.Support
{
    /// <summary>
    /// ApplicationContext that can scan to identify object definitions
    /// </summary>
    public class ScanningApplicationContext : GenericApplicationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Spring.Context.Support.GenericApplicationContext"/> class.
        /// </summary>
        public ScanningApplicationContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Spring.Context.Support.GenericApplicationContext"/> class.
        /// </summary>
        /// <param name="caseSensitive">if set to <c>true</c> names in the context are case sensitive.</param>
        public ScanningApplicationContext(bool caseSensitive) : base(caseSensitive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Spring.Context.Support.GenericApplicationContext"/> class.
        /// </summary>
        /// <param name="objectFactory">The object factory instance to use for this context.</param>
        public ScanningApplicationContext(DefaultListableObjectFactory objectFactory) : base(objectFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Spring.Context.Support.GenericApplicationContext"/> class.
        /// </summary>
        /// <param name="parent">The parent application context.</param>
        public ScanningApplicationContext(IApplicationContext parent) : base(parent)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Spring.Context.Support.GenericApplicationContext"/> class.
        /// </summary>
        /// <param name="name">The name of the application context.</param><param name="caseSensitive">if set to <c>true</c> names in the context are case sensitive.</param><param name="parent">The parent application context.</param>
        public ScanningApplicationContext(string name, bool caseSensitive, IApplicationContext parent) : base(name, caseSensitive, parent)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Spring.Context.Support.GenericApplicationContext"/> class.
        /// </summary>
        /// <param name="objectFactory">The object factory to use for this context</param><param name="parent">The parent applicaiton context.</param>
        public ScanningApplicationContext(DefaultListableObjectFactory objectFactory, IApplicationContext parent) : base(objectFactory, parent)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Spring.Context.Support.GenericApplicationContext"/> class.
        /// </summary>
        /// <param name="name">The name of the application context.</param><param name="caseSensitive">if set to <c>true</c> names in the context are case sensitive.</param><param name="parent">The parent application context.</param><param name="objectFactory">The object factory to use for this context</param>
        public ScanningApplicationContext(string name, bool caseSensitive, IApplicationContext parent, DefaultListableObjectFactory objectFactory) : base(name, caseSensitive, parent, objectFactory)
        {
        }

        public void Scan(AssemblyObjectDefinitionScanner scanner)
        {
            scanner.ScanAndRegisterTypes(DefaultListableObjectFactory);
        }

        public void Scan()
        {
            Scan(new AssemblyObjectDefinitionScanner());
        }

        public  void Scan(Predicate<Type> typePredicate)
        {
            Scan(null, ta => true, typePredicate);
        }

        public  void Scan(string assemblyScanPath, Predicate<Assembly> assemblyPredicate, Predicate<Type> typePredicate)
        {
            //create a scanner instance using the scan path
            AssemblyObjectDefinitionScanner scanner = new AssemblyObjectDefinitionScanner(assemblyScanPath);

            //configure the scanner per the provided constraints
            scanner.WithAssemblyFilter(assemblyPredicate).WithIncludeFilter(typePredicate);

            //pass the scanner to primary Scan method to actually do the work
            Scan(scanner);
        }

        public void Scan(Predicate<Assembly> assemblyPredicate, Predicate<Type> typePredicate)
        {
            Scan(null, assemblyPredicate, typePredicate);
        }

        public  void Scan(Predicate<Assembly> assemblyPredicate)
        {
            Scan(null, assemblyPredicate, t => true);
        }


       

    }
}
