#!/bin/bash
# Script to set up development environment for Unity Meta Quest 3 Test project
# This script installs all necessary dependencies for Unity development on macOS

echo "Setting up development environment for Unity Meta Quest 3 Test..."

# Check if Homebrew is installed, install if not
if ! command -v brew &> /dev/null; then
    echo "Homebrew not found. Installing Homebrew..."
    /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
else
    echo "Homebrew already installed. Updating..."
    brew update
fi

# Check if Unity Hub is installed, install if not
if ! ls /Applications/Unity\ Hub.app &> /dev/null; then
    echo "Unity Hub not found. Installing Unity Hub..."
    brew install --cask unity-hub
else
    echo "Unity Hub already installed"
fi

# Install other development tools
echo "Installing development tools..."
brew install git-lfs # For handling large Unity assets
brew install --cask visual-studio-code # IDE for script editing

# Check if Android SDK and JDK are installed for Meta Quest development
if ! command -v adb &> /dev/null; then
    echo "Android SDK not found. Installing Android command-line tools..."
    brew install --cask android-commandlinetools
fi

# Suggest installing JDK if not present
if ! command -v java &> /dev/null; then
    echo "JDK not found. Installing OpenJDK..."
    brew install openjdk
fi

echo "Development environment setup complete!"
echo "Please open Unity Hub and install Unity version 2022.3 LTS or newer with Android Build Support"
echo "Then, in Unity Hub, add Meta Quest XR Plugin via the Package Manager"

# Make the script executable
chmod +x "$(dirname "$0")/setup_dev_environment.sh"