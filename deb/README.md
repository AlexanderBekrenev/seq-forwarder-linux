# Seq Forwarder (fork for Linux)
## Reason
The reason for creating this fork was due to several reasons why the original code was not easy to run as a service under Linux.
The original code contains everything necessary to run it as a service under Windows. 
But Windows service code is not suitable for Linux. 

Running Seq Forwarder as a console program works well under Linux.  
If you are comfortable with this launch option, if you can run a console program, 
for example using Docker or other services, then it is likely that you do not need this fork.

I wanted Seq Forwarder to be its own daemon controlled by Systemd.

## What's the result?

* Systemd service **seqfwd**
* Installation using Debian package (segfwd-...-amd64.deb)
* Ability to assemble a custom deb-package with a preset Sec server URL (segfwd-custom-...-amd64.deb)

If you are not very interested in what was changed and what was done, then feel free to skip to the [How-to](#How-to) chapter.

## Original code changes

Major changes affected the `RunCommand.cs` file.

System support added (`UseSystemd()`).  
Monitoring the pressing of Ctrl-C is done via `UseConsoleLifetime()`.  
Added the ability to set parameters from `ServerInformationFeature` at startup. These are `-url` and `-apikey`.  
When running as a service, error logging has been changed. Only FATAL level errors are written to the console. They will end up in syslog.

Fixed a bug with setting the configuration parameter `diagnostics.internalLoggingLevel` (ConfigCommand.cs)

## Deb-package

Everything related to the deb package is located in the `/deb` directory.  
Actually, this is a directory with a template for assembling the package (`/deb/seqfwd.template`) and a script for assembling it (`/deb/builddeb.sh`).

If you are not familiar with the principle of building deb packages,
then it is better to find a description of this process.  
In short, the template consists of installation control scripts under directory DEBIAN
and the actual package files.

The script for assembling the package publishes the project `Seq.Forwarder` to the intermediate folder `/deb/publish.seqfwd`.
A temporary folder for building the package is created, into which the contents of the package template and the contents of the intermediate folder are copied.
The necessary substitutions are made in the data and a deb package is assembled.


Key points of the installed service:
* Service name is `seqfwd` or full name is `seqfwd.service`
* Service file is `/lib/systemd/system/seqfwd.service`
* Executed as user `seqfvd`. The user is created when the package is installed.
* Code directory is `/usr/lib/seqfwd`
* Working directory is `/var/lib/seqfwd/`. Folder `Buffer` and file `SeqForwarder.json` are there.
* Log directory is `/var/log/seqfwd`
* Service settings are stored in a file `/etc/systemd/system/seqfwd.service.d/override.conf`

## How-to
### Build DEB-package

* Open the console in the `/deb` folder.
* Run the script `./builddeb.sh`
* The package is ready: `seqfwd-2.1.128.2-amd64.deb`

If you want to make a custom package, you can set the Sec server URL using the script parameter.
For example, run `./builddeb.sh http://192.168.10.3:5341`  
As a result, we will get a packet `seqfwd-custom-2.1.128.2-amd64.deb` with a preset address.

### Install service

Install the deb package on the server in any way convenient for you.  
You may be using your own repository.  
Alternatively, you can simply copy the package file to the server and 
install using the `dpkg -i seqfwd-2.1.128.2-amd64.deb` command.

If you created a custom package and do not use API keys, then the service is ready to use.  
Otherwise, you need to configure it.

Setting up the service involves editing the file `/etc/systemd/system/seqfwd.service.d/override.conf`.
Use standard editing capabilities instead of editing directly.

`systemctl edit seqfwd.service`

You may want to use a simple `nano` editor instead of the `vi` editor that is usually the default.

`EDITOR=nano systemctl edit seqfwd.service`

Edits must be made in the last line of the text below.
```
#TODO set real address of Seq server and API key
#ExecStart=/usr/lib/seqfwd/seqfwd run -s=/var/lib/seqfwd/ -u=http://localhost:5341 --apikey=KEYVALUE
ExecStart=/usr/lib/seqfwd/seqfwd run -s=/var/lib/seqfwd/ --url=http://localhost:5341
```

Just in case, restart the service after edits.

`systemctl restart seqfwd`
