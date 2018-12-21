Dapplo.Frickler
===============

The German word "Frickeln" roughly translated means someone who is tinkering or fiddling (around).
This tool helps me to tinker around in an environment where a corporate proxy makes it hard to use some of the tools I want to use.

The concept is very easy, by default the application starts FiddlerCore as a proxy and makes it possible to have applications which support proxies or use the default proxy, to be correctly authenticated.
For some applications you need to modify the environment variables HTTP_PROXY and or HTTPS_PROXY, this can be done automatically.

The application runs in the background, creates a system tray icon, where you can exit or open the configuration. It also informs you via a popup of changes to the internet settings, for instance while group policies are applied, where it automatically restarts to accommodate for them.