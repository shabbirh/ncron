﻿/*
 * Copyright 2008, 2009 Joern Schou-Rode
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace NCron.Service
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            Installers.Add(new ServiceProcessInstaller
                               {
                                   Account = ServiceAccount.LocalService
                               });

            Installers.Add(new ServiceInstaller
                               {
                                   ServiceName = "NCron",
                                   DisplayName = "NCron Scheduler",
                                   Description = "Executes jobs according to the configured NCron schedule.",
                                   StartType = ServiceStartMode.Automatic
                               });
        }

        internal static void Install(bool undo)
        {
            var assembly = typeof(ProjectInstaller).Assembly;

            using (var installer = new AssemblyInstaller { Assembly = assembly, UseNewContext = true })
            {
                var state = new Hashtable();

                try
                {
                    if (undo)
                    {
                        installer.Uninstall(state);
                    }
                    else
                    {
                        installer.Install(state);
                        installer.Commit(state);
                    }
                }
                catch
                {
                    try
                    {
                        installer.Rollback(state);
                    }
                    catch { }
                    throw;
                }
            }
        }
    }
}
