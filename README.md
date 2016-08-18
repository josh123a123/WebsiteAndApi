# DevSpace
This is the website used for the DevSpace Technical Conference in Huntsville, Alabama.

The project is a .Net WebAPI project.

The website itself uses SSI for templating. I have the SSI processor running on the standard html pages, so note the special line added to the web.config to add this.

There is a file that is in the .getignore file called analytics.js.  It is the file that contains our Google Analytics code. I'm not worried about people stealing it. You could just grab it off the rendered webpage. However, I don't want people forking the code, forgetting it's there, and acidentially giving us fake analytics.
