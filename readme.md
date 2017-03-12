# Heras
UWP app built for Windows 10 iot core. Tested on a Raspberry Pi 3 with the official 7" touchscreen (800x480px).

![Logo: Heos + Raspberry Pi = Heras](heos+raspberry+heras.png)
## Missing features 
1. All commands except for get currently playing and play next.
2. Speakers are not fetched as part of the app. You have to run the get_players command manually.

## Setup

1. Add your own Hockey App key in ```App.xaml.cs```
2. Change the hardcoded ```pid``` to a pid matching one of your Heos devices. Can be found by running the get_players command manually.
