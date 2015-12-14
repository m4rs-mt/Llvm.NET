﻿using System;
using System.IO;

namespace Llvm.NET
{
    /// <summary>LLVM MemoryBuffer</summary>
    public sealed class MemoryBuffer
        : IDisposable
    {
        /// <summary>Load a file as an LLVM Memory Buffer</summary>
        /// <param name="path">Path of the file to load into a <see cref="MemoryBuffer"/></param>
        public MemoryBuffer( string path )
        {
            if( string.IsNullOrWhiteSpace( path ) )
                throw new ArgumentException( "Path cannot be null or an empty string", nameof( path ) );

            if( !File.Exists( path ) )
                throw new FileNotFoundException( "Specified file not found", path );

            IntPtr msg;
            if( NativeMethods.CreateMemoryBufferWithContentsOfFile( path, out BufferHandle_, out msg ).Succeeded )
                return;

            throw new InternalCodeGeneratorException( NativeMethods.MarshalMsg( msg ) );
        }

        /// <summary>Size of the buffer</summary>
        int Size
        {
            get
            {
                if( BufferHandle.Pointer == IntPtr.Zero )
                    return 0;

                return NativeMethods.GetBufferSize( BufferHandle );
            }
        }

        public void Dispose( )
        {
            if( BufferHandle.Pointer != IntPtr.Zero )
            {
                NativeMethods.DisposeMemoryBuffer( BufferHandle );
                BufferHandle_ = default(LLVMMemoryBufferRef);
            }
        }

        internal LLVMMemoryBufferRef BufferHandle => BufferHandle_;
        
        // keep as a private field so this is usable as an out param in constructor
        private LLVMMemoryBufferRef BufferHandle_;
    }
}
