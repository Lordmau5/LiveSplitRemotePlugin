# LiveSplit Remote Plugin

Livesplit Remote allows an instance of Livesplit on one PC to control another instance of Livesplit on another PC in order to sync Time values and split events.
This is helpful for runners using a 2 PC setup (a Game PC and a Stream PC) to maintain highly accurate IGT or RTA no-load times with support for autosplitters, without the hassle a complex capture setup.
It may also be useful in situation like races, where participants could all send thier local timer telemetry to a offsite broadcaster. 

In addition to using LiveSplit Remote on your Game PC, you'll also need Livesplit Server on your Stream PC to so it can receive the telemetry.

This plugin was based off of the https://github.com/Villhellm/LiveSplitClientPlugin/ .  The core differences are: 
* Livesplit Remote runs on the Livesplit of the **Game PC**, not the **Stream PC**, and _pushes_ time values and status/split changes to the Stream PC
* this plugin listens to the Start, Reset, Pause, and Resume events from Livesplit and relays these to the server
* When the Autosplitting setting is disabled, split controls are synced through events, providing more consistent support for skipping and reversing splits when using manual split controls

## PREREQUISITES

LiveSplit: https://github.com/LiveSplit/LiveSplit/releases

LiveSplit Server: https://github.com/LiveSplit/LiveSplit.Server/releases

LiveSplit Remote: https://github.com/jayo-exe/LiveSplitRemotePlugin/releases

## SETUP

### STREAMING PC:

1. Download LiveSplit and the LiveSplit Server plugin.
2. Add the plugin to your layout (Right-click > Edit Layout > + > Control > LiveSplit Server) Save all settings.
3. Add IGT or RTA no-load plugin for whatever game you are playing and set the timing method to "Game Time" (Right-click > Compare Against > Game Time)
4. Start the server (Right-click > Controls > Start Server) and start the timer (Right-click > Controls > Start)

### GAMING PC:

1. Download LiveSplit and the Livesplit Remote plugin
2. Add the plugin to your layout (Right-click > Edit Layout > + > Control > Live Split Remote)
3. Set the IP address of the server you set up (Layout Settings > LiveSplit Remote tab > IP address)
4. Press the "Attempt Connection" button. It should say "Connected" under server status. If not make sure your IP address and port are correct and that the server is running. Save all settings.
5. Set the timing method to "Game Time" (Right-click > Compare Against > Game Time) and start the timer (Right-click > Controls > Start)

Your timers are now synced! Any changes on the Gaming PC timer will be sent to the server and should be reflected on the Streaming PC timer.
All actual splits should be saved on the Streaming PC. The gaming side is just for the game timer.


## NOTES

To install plugins just add the .dll files to the "Components" folder in the LiveSplit folder.

Make sure to save the layouts after you set everything up so you don't have to do it every time you launch.

When launching the Livesplit on the Streaming PC, remember to start the server! This doesn't happen automatically

## TROUBLESHOOTING

After making sure all steps in the setup were followed, make sure there is no _unexpected_ programs or plugins controlling the timer on the Stream PC or the Game PC. 

Given the nature of how this plugin syncs Game Time, this should be compatible with most autosplitters and similar plugins that modify the timer value.  Just be sure thiese are running on the Game PC/Remote!

If it says "Connected" in the settings menu but the times don't match, make sure the Streaming PC **"Compare Against"** is set to **"Game Time"**.
