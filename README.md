# OriginalMainMenu

A simple unintrusive mod to return the original main menu of Sunless Sea when Zubmariner is installed, instead of forcing the Zubmariner background.

## Features

- Toggleable title card
- Toggleable menu background
 
## Configuration

Edit `OriginalMainMenu.ini` in your Sunless Sea **install** folder to customize:

- `Replace Title = true/false` - Whether to replace the "Sunless Sea Zubmariner" title card with the original
- `Replace Background = true/false` - Whether to replace the sea mine background with the buoy background

**Note**: Setting these to true while not having Zubmariner installed will do nothing (possible exception for the Epic Games version).

## **Compiling the plugin**

To develop and build the plugin, there are a couple of prerequisites. Clone the repository:

```bash
git clone https://github.com/MagicJinn/SSea-OriginalMainMenu.git
cd SSea-OriginalMainMenu
```

After this, you need to acquire a DLL OriginalMainMenu relies on. Create a `dependencies` folder, and find `Sunless.Game.dll` in your `SunlessSea\Sunless Sea_Data\Managed` folder. Copy it into the `dependencies` folder. After this, you should be able to compile the project with the following command:

```bash
dotnet build -c Release -p:Optimize=true
```

The DLL should be located in `bin/Release/net35`.
