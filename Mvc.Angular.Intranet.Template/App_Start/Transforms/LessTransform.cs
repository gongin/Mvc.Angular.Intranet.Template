using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Optimization;
using dotless.Core;
using dotless.Core.Abstractions;
using dotless.Core.Importers;
using dotless.Core.Input;
using dotless.Core.Loggers;
using dotless.Core.Parser;

namespace Mvc.Angular.Intranet.Template
{
    public class LessTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse bundle)
        {
            context.HttpContext.Response.Cache.SetLastModifiedFromFileDependencies();

            var lessParser = new Parser();
            ILessEngine lessEngine = CreateLessEngine(lessParser);
            CustomPathProvider pathProvider = new CustomPathProvider();
            var content = new StringBuilder(bundle.Content.Length);

            var bundleFiles = new List<BundleFile>();

            // Fix the path before attempting to resolve
            SetCurrentFilePath(lessParser, HttpRuntime.AppDomainAppPath);
            IPathResolver pathResolver = GetPathResolver(lessParser);

            foreach (var bundleFile in bundle.Files)
            {
                bundleFiles.Add(bundleFile);

                var virtualFilePath = bundleFile.VirtualFile.VirtualPath;
                var filePath = pathResolver.GetFullPath(virtualFilePath);

                SetCurrentFilePath(lessParser, filePath);


                using (var stream = new StreamReader(bundleFile.VirtualFile.Open()))
                {
                    string source = stream.ReadToEnd();
                    content.Append(lessEngine.TransformToCss(source, filePath));
                    content.AppendLine();
                }

                bundleFiles.AddRange(GetFileDependencies(lessParser));
            }

            if (BundleTable.EnableOptimizations)
            {
                // include imports in bundle files to register cache dependencies
                bundle.Files = bundleFiles.Distinct();
            }

            bundle.ContentType = "text/css";
            bundle.Content = content.ToString();
        }

        /// <summary>
        /// Creates an instance of LESS engine.
        /// </summary>
        /// <param name="lessParser">The LESS parser.</param>
        private ILessEngine CreateLessEngine(Parser lessParser)
        {
            var logger = new AspNetTraceLogger(LogLevel.Debug, new Http());
            return new LessEngine(lessParser, logger, true, false);
        }

        /// <summary>
        /// Gets the file dependencies (@imports) of the LESS file being parsed.
        /// </summary>
        /// <param name="lessParser">The LESS parser.</param>
        /// <returns>An array of file references to the dependent file references.</returns>
        private IEnumerable<BundleFile> GetFileDependencies(Parser lessParser)
        {
            IPathResolver pathResolver = GetPathResolver(lessParser);

            foreach (var importPath in lessParser.Importer.Imports)
            {
                var s = pathResolver.GetFullPath(importPath);

                CustomPathProvider pathProvider = new CustomPathProvider();
                var b = new BundleFile(importPath, pathProvider.GetFile(s));
                yield return b;
            }

            lessParser.Importer.Imports.Clear();
        }

        /// <summary>
        /// Returns an <see cref="IPathResolver"/> instance used by the specified LESS lessParser.
        /// </summary>
        /// <param name="lessParser">The LESS parser.</param>
        private IPathResolver GetPathResolver(Parser lessParser)
        {
            var importer = lessParser.Importer as Importer;
            var fileReader = importer.FileReader as FileReader;

            return fileReader.PathResolver;
        }

        /// <summary>
        /// Informs the LESS parser about the path to the currently processed file. 
        /// This is done by using a custom <see cref="IPathResolver"/> implementation.
        /// </summary>
        /// <param name="lessParser">The LESS parser.</param>
        /// <param name="currentFilePath">The path to the currently processed file.</param>
        private void SetCurrentFilePath(Parser lessParser, string currentFilePath)
        {
            var importer = lessParser.Importer as Importer;

            if (importer == null)
                throw new InvalidOperationException("Unexpected dotless importer type.");

            var fileReader = new FileReader(new ImportedFilePathResolver(currentFilePath));
            importer.FileReader = fileReader;
        }
    }

    public class CustomPathProvider : VirtualPathProvider
    {
        public CustomPathProvider()
            : base()
        {
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return null;
        }

        public override bool FileExists(string virtualPath)
        {
            return base.FileExists(virtualPath);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return new CustomVirtualFile(virtualPath);
        }

        public class CustomVirtualFile : System.Web.Hosting.VirtualFile
        {
            string path;

            public CustomVirtualFile(string path)
                : base(path)
            {
                //deal with this later
                this.path = path;
            }

            public override System.IO.Stream Open()
            {
                return VirtualPathProvider.OpenFile(path);
            }
        }
    }

    public class ImportedFilePathResolver : IPathResolver
    {
        private string currentFileDirectory;
        private string currentFilePath;

        public ImportedFilePathResolver(string currentFilePath)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                throw new ArgumentNullException("currentFilePath");
            }

            CurrentFilePath = currentFilePath;
        }

        /// <summary>
        /// Gets or sets the path to the currently processed file.
        /// </summary>
        public string CurrentFilePath
        {
            get { return currentFilePath; }
            set
            {
                currentFilePath = value;
                currentFileDirectory = Path.GetDirectoryName(value);
            }
        }

        /// <summary>
        /// Returns the absolute path for the specified improted file path.
        /// </summary>
        /// <param name="filePath">The imported file path.</param>
        public string GetFullPath(string filePath)
        {
            if (filePath.StartsWith("~"))
            {
                filePath = VirtualPathUtility.ToAbsolute(filePath);
            }

            if (filePath.StartsWith("/"))
            {
                filePath = HostingEnvironment.MapPath(filePath);
            }
            else if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.GetFullPath(Path.Combine(currentFileDirectory, filePath));
            }

            return filePath;
        }
    }
}