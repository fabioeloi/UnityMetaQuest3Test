#!/bin/bash
# Script to deploy Unity build to Meta Quest 3 device
# This is part of our IaC approach for streamlining the development workflow

# Configuration
APK_PATH=${1:-"./Unity/build/Android/UnityMetaQuest3Test.apk"}
DEVICE_ID=${2:-""}
APP_ID="com.fabioeloi.unitymetaquest3test"

echo "Meta Quest 3 Deployment Script"
echo "==============================="

# Check if adb is installed
if ! command -v adb &> /dev/null; then
    echo "Error: ADB is not installed. Please run setup_dev_environment.sh first."
    exit 1
fi

# Check if APK exists
if [ ! -f "$APK_PATH" ]; then
    echo "Error: APK file not found at $APK_PATH"
    exit 1
fi

# Check for connected devices
echo "Checking for connected Quest devices..."
DEVICES=$(adb devices | grep -v "List" | grep "device" | wc -l)

if [ "$DEVICES" -eq 0 ]; then
    echo "Error: No Meta Quest devices found. Please make sure your Quest is:"
    echo "  1. Connected via USB cable"
    echo "  2. Has USB debugging enabled"
    echo "  3. You've allowed USB debugging for this computer"
    exit 1
fi

# If multiple devices are connected and no specific device ID was provided
if [ "$DEVICES" -gt 1 ] && [ -z "$DEVICE_ID" ]; then
    echo "Multiple devices found. Please specify a device ID:"
    adb devices
    exit 1
fi

# Set device ID flag if provided
DEVICE_FLAG=""
if [ ! -z "$DEVICE_ID" ]; then
    DEVICE_FLAG="-s $DEVICE_ID"
fi

# Check if app is already installed
echo "Checking if app is already installed..."
APP_INSTALLED=$(adb $DEVICE_FLAG shell pm list packages | grep $APP_ID)

if [ ! -z "$APP_INSTALLED" ]; then
    echo "App is already installed. Uninstalling previous version..."
    adb $DEVICE_FLAG uninstall $APP_ID
fi

# Install APK to device
echo "Installing $APK_PATH to Quest device..."
adb $DEVICE_FLAG install -r "$APK_PATH"

if [ $? -eq 0 ]; then
    echo "Installation successful!"
    
    # Launch the app
    echo "Launching app on Quest..."
    adb $DEVICE_FLAG shell am start -n $APP_ID/com.unity3d.player.UnityPlayerActivity
    
    echo "Deployment complete!"
else
    echo "Installation failed."
    exit 1
fi