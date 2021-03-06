﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Common.Tools.DotCover.Cover
{
    /// <summary>
    /// DotCover Coverer builder.
    /// </summary>
    public sealed class DotCoverCoverer : DotCoverTool<DotCoverCoverSettings>
    {
        private readonly ICakeEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotCoverCoverer" /> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="processRunner">The process runner.</param>
        /// <param name="tools">The tool locator.</param>
        public DotCoverCoverer(
            IFileSystem fileSystem,
            ICakeEnvironment environment,
            IProcessRunner processRunner,
            IToolLocator tools) : base(fileSystem, environment, processRunner, tools)
        {
            _environment = environment;
        }

        /// <summary>
        /// Runs DotCover Cover with the specified settings.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="action">The action.</param>
        /// <param name="outputPath">The output file path.</param>
        /// <param name="settings">The settings.</param>
        public void Cover(ICakeContext context,
            Action<ICakeContext> action,
            FilePath outputPath,
            DotCoverCoverSettings settings)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            if (outputPath == null)
            {
                throw new ArgumentNullException(nameof(outputPath));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            // Run the tool using the interceptor.
            var interceptor = InterceptAction(context, action);

            // Run the tool.
            Run(settings, GetArguments(interceptor, settings, outputPath));
        }

        private static DotCoverContext InterceptAction(
            ICakeContext context,
            Action<ICakeContext> action)
        {
            var interceptor = new DotCoverContext(context);
            action(interceptor);

            // Validate arguments.
            if (interceptor.FilePath == null)
            {
                throw new CakeException("No tool was started.");
            }

            return interceptor;
        }

        private ProcessArgumentBuilder GetArguments(
            DotCoverContext context,
            DotCoverCoverSettings settings,
            FilePath outputPath)
        {
            var builder = new ProcessArgumentBuilder();

            builder.Append("Cover");

            // The target application to call.
            builder.AppendSwitch("/TargetExecutable", "=", context.FilePath.FullPath.Quote());

            // The arguments to the target application.
            var arguments = context.Settings?.Arguments?.Render();
            if (!string.IsNullOrWhiteSpace(arguments))
            {
                arguments = arguments.Replace("\"", "\\\"");
                builder.AppendSwitch("/TargetArguments", "=", arguments.Quote());
            }

            // Set the output file.
            outputPath = outputPath.MakeAbsolute(_environment);
            builder.AppendSwitch("/Output", "=", outputPath.FullPath.Quote());

            // Set common Coverage settings
            settings.ToArguments(_environment).CopyTo(builder);

            return builder;
        }
    }
}