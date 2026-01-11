# Grace Gawker
[![Version number](https://img.shields.io/badge/version-1.1.6.0-ff6262)](https://github.com/mashirochan/GraceGawker)
[![Download count](https://img.shields.io/endpoint?url=https%3A%2F%2Fqzysathwfhebdai6xgauhz4q7m0mzmrf.lambda-url.us-east-1.on.aws%2FGraceGawker&color=%23ff6262)](https://github.com/mashirochan/GraceGawker)
[![Availability](https://img.shields.io/badge/availability-stable-limegreen)](https://github.com/mashirochan/GraceGawker)

![H4neEFo](https://github.com/user-attachments/assets/d862b418-71fc-46f6-a14a-846860545858)

A simple plugin that tracks the remaining EXP left on your Crafter and Gatherer's Grace buffs.

Please not that this plugin simply attempts to give *a rough estimate*. The exact EXP remaining on the buff is not accessible, so all the plugin can do is estimate based off of EXP gains whilst crafting or gathering. Thus, the display value is only properly estimated when a fresh Grace buff is activated after the plugin is loaded.

For any questions or bugs, please [Create an Issue](https://github.com/mashirochan/FFXIV-GraceGawker/issues/new/choose).

## Configuration

* Enabled: Whether or not the Main Window is visible `(Default: YES)`
* Movable: Whether or not the Main Window is movable `(Default: NO)`
* Display Settings
  * Hide for Wrong Jobs: Hides the Main Window if the currently selected Job does not benefit from the current Grace buff `(Default: NO)`
* Format Settings
  * Show Manual Name: Whether or not the name of the current Manual should be displayed or not `(Default: YES)`
  * Show Percent: Whether or not the percent remaining of the Grace EXP buff should be displayed or not `(Default: YES)`
  * Show Percent On Bar: Whether the progress bar should display the percent remaining of the Grace EXP buff, or display the numerical value of the remaining and max EXP `(Default: NO)`

## How To Use

### Prerequisites

Grace Gawker assumes all the following prerequisites are met:

* XIVLauncher, FINAL FANTASY XIV, and Dalamud have all been installed and the game has been run with Dalamud at least once.
* XIVLauncher is installed to its default directories and configurations.

### Activating in-game

1. Launch the game and use `/xlsettings` in chat or `xlsettings` in the Dalamud Console to open up the Dalamud settings.
    * In here, go to `Experimental`, and add the full path to the `SamplePlugin.dll` to the list of Dev Plugin Locations.
2. Next, use `/xlplugins` (chat) or `xlplugins` (console) to open up the Plugin Installer.
    * In here, go to `All Plugins`, and `Grace Gawker` should be visible. Enable it.
3. You should now be able to use `/pgrace` (chat) or `pgrace` (console) to open the Grace Gawker config!
	* *Note: the plugin can only track Grace buffs if a fresh manual has been activated after enabling the plugin.*
