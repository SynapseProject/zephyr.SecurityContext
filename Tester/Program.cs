﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using zephyr.SecurityContext;
using zephyr.SecurityContext.Windows;

namespace ImpersonationTester
{
    class Program
    {
        static void Main(string[] args)
        {
            if( args.Length == 0 )
            {
                Console.WriteLine( $"Syntax: runasx inputfile" );

                if( !File.Exists( "sample.yaml" ) )
                {
                    RunInfo.CreateSample();
                    Console.WriteLine( $"Created sample inputfile: sample.yaml" );
                }

                Environment.Exit( 0 );
            }

            string path = args[0];
            if( !File.Exists( path ) )
            {
                Console.WriteLine( $"Could not find file {path}." );
                Environment.Exit( 1 );
            }

            RunInfo r = RunInfo.DeserializeFile( path );

            Process process = new Process
            {
                StartInfo = r.StartInfo.ToProcessStartInfo()
            };

            if( r.HasProcessRunAs && r.ProcessRunAs.UseIdentity )
            {
                process.StartInfo.Domain = r.ProcessRunAs.Domain;
                process.StartInfo.UserName = r.ProcessRunAs.UserName;
                process.StartInfo.Password = r.ProcessRunAs.Password.ToSecureString();
            }

            try
            {
                StringBuilder stdout = new StringBuilder();

                Win32Identity win32Identity = null;
                if( r.HasIdentityRunAs && r.IdentityRunAs.UseIdentity )
                {
                    win32Identity = new Win32Identity();
                    win32Identity.Impersonate( r.IdentityRunAs.UserName.ToSecureString(), r.IdentityRunAs.Domain.ToSecureString(), r.IdentityRunAs.Password.ToSecureString() );
                }

                process.Start();

                Thread stdOutReader = new Thread( delegate ()
                {
                    while( !process.StandardOutput.EndOfStream )
                    {
                        string line = process.StandardOutput.ReadLine();
                        lock( stdout )
                        {
                            stdout.AppendLine( line );
                        }
                    }
                } );
                stdOutReader.Start();

                Thread stdErrReader = new Thread( delegate ()
                {
                    while( !process.StandardError.EndOfStream )
                    {
                        string line = process.StandardError.ReadLine();
                        lock( stdout )
                        {
                            stdout.AppendLine( line );
                        }
                    }
                } );
                stdErrReader.Start();

                process.WaitForExit( r.StartInfo.TimeoutMilliseconds );

                win32Identity?.Undo();

                Console.WriteLine( stdout.ToString() );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex );
                Environment.Exit( 1 );
            }
        }
    }
}