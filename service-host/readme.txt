This is a service process host that allows you to run any application as a service
under windows 7 / server 2008 or above.  Windows resource kit used to provide utilities
that did the same thing, but afaik those don't work with the new OS and microsoft
hasn't released anything similar.

The problem with running any old program as a service is that there is no
interactive desktop session for a cmd.exe window and there are no stderr /
stdout streams for your typical non-gui program.  This is a launcher that redirects
those stderr / stdout streams to a single file.  It has a couple other frills I
felt like throwing in such as size limits similar log4net's RollingFileAppender
and a minimal file locking scheme so that you don't have to stop the service to
take a peek at the output.

You can use sc.exe to create new services.
