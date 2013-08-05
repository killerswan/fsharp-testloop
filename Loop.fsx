(* Copyright (c) 2013, Kevin Cantu <me@kevincantu.org>
 *
 * Permission to use, copy, modify, and/or distribute this software for any
 * purpose with or without fee is hereby granted, provided that the above
 * copyright notice and this permission notice appear in all copies.
 *
 * The software is provided "as is" and the author disclaims all warranties
 * with regard to this software including all implied warranties of
 * merchantability and fitness. In no event shall the author be liable for
 * any special, direct, indirect, or consequential damanges or any damages
 * whatsoever resulting from loss of use, data, or profits, whether in an
 * action of contract, negligence, or other tortious action, arising out of
 * or in connection with the use or performance of this software.
 *)

open System
open System.IO
open System.Diagnostics
open System.Windows.Forms

module Loop =
   let runCmd (command : string) (args : string) : Process =
      let ps = new Process()
      ps.StartInfo.FileName <- command
      ps.StartInfo.Arguments <- args
      ps.StartInfo.UseShellExecute <- false
      ps.StartInfo.RedirectStandardOutput <- true

      printfn "======================================="
      printfn "%s" (System.DateTime.Now.ToString())
      printfn "command: %A" command
      printfn "args: %A" args
      let running = ps.Start()

      printfn "======================================="
      printfn ""

      printfn "%O" (ps.StandardOutput.ReadToEnd())
      //printfn "[%d]" (ps.ExitCode)
      ps
    
   let fs_nunit extra_args =
      let nu_console = "/Library/Frameworks/Mono.framework/Versions/3.2.1/lib/mono/4.5/nunit-console.exe"
      let mono_opts = Environment.GetEnvironmentVariable("MONO_OPTIONS")

      printfn "NUnit..."
      let args = "--debug " + mono_opts + " " + nu_console + " -nologo -nodots " + extra_args
      runCmd "/usr/bin/mono" args

   let fs_nunit_dll dll =
      if (File.Exists(dll))
      then
         fs_nunit dll |> ignore
      else
         ()

   let compile (files : seq<string>) : unit =
      let fsc file =
         let aa = "--nologo --target:library " + file
         runCmd "/usr/bin/fsharpc" aa |> ignore 

      printfn "fsc..."
      files |> Seq.iter fsc

   let addWatcherForFiles (dir : string) (filter : string) (action : System.IO.FileSystemEventArgs -> unit) : unit =

      printfn "Adding a watcher for %A" filter

      let watcher =
         new System.IO.FileSystemWatcher(Path = dir, Filter = filter)

      watcher.Changed.Add action
      watcher.Created.Add action
      watcher.Deleted.Add action
       
      watcher.EnableRaisingEvents <- true

      ()

   let watchFilesWithAction (filters : seq<string>) (action : System.IO.FileSystemEventArgs -> unit) : unit =
      filters
      |> Seq.iter (fun filt -> addWatcherForFiles "." filt action)

   let watchFiltersAndCompileScript (filters : seq<string>) (mainScript : string) =
      let act (evtArgs : System.IO.FileSystemEventArgs) : unit =
         compile [mainScript] |> ignore
      watchFilesWithAction filters act

   let watchAndRunTestDLLs (libs : seq<string>) : unit =
      let act (evtArgs : System.IO.FileSystemEventArgs) : unit =
         fs_nunit_dll (evtArgs.FullPath)
      watchFilesWithAction libs act
   
   let run () =
      Application.Run()
