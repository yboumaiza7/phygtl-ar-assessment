# AR Assessment by Yassine

A professional-grade Unity AR application demonstrating advanced AR development skills, architectural design patterns, and production-ready code quality. Built with Unity 6 and AR Foundation for cross-platform AR experiences.

## 📋 Table of Contents

- [🎯 Project Overview](#-project-overview)
- [✨ Key Features & Technical Highlights](#-key-features--technical-highlights)
- [🏗️ Architecture & Design Patterns](#️-architecture--design-patterns)
- [🔧 Technical Implementation](#-technical-implementation)
- [📋 Prerequisites](#-prerequisites)
- [🚀 Setup & Installation](#-setup--installation)
- [🎮 Usage Guide](#-usage-guide)
- [📁 Project Structure](#-project-structure)
- [💎 Code Quality & Best Practices](#-code-quality--best-practices)
- [⚡ Performance Optimizations](#-performance-optimizations)
- [🔨 Build Instructions](#-build-instructions)
- [🎓 Skills Demonstrated](#-skills-demonstrated)
- [📞 Contact](#-contact)
- [📄 License](#-license)

---

## 🎯 Project Overview

This project is a **skill assessment** demonstrating comprehensive AR development capabilities, including:

- ✅ **Core AR Functionality**: Plane detection, object placement, and real-time interaction
- ✅ **Advanced UI Development**: Custom animated placement wheel with DOTween integration
- ✅ **Backend Architecture**: Robust download system with caching and progress tracking
- ✅ **Cloud Connectivity**: Dynamic GLTF asset loading from remote servers
- ✅ **Production-Ready Code**: Clean architecture, design patterns, and comprehensive documentation

**Purpose**: Standalone assessment project for Upwork client interview process  
**Status**: ✅ Complete and ready for evaluation  
**Unity Version**: 6000.0.58f2 or later

---

## ✨ Key Features & Technical Highlights

### 🎮 Core AR Features

#### 1. **Intelligent Plane Detection & Visualization**
- Real-time horizontal surface detection using ARFoundation
- Configurable plane alignment preferences (horizontal/vertical)
- Smooth fade-in/fade-out animations for plane visualizers
- Debug visualization options with AR Debug Menu integration

#### 2. **Advanced Object Placement System**
- **Placement Wheel UI**: Custom radial menu for object selection
  - Animated reveal/hide with DOTween
  - Dynamic slot generation based on available objects
  - Visual feedback for download status (loading, cached, error)
  - Precise click detection with calculated bounding boxes
- **Smart Positioning**: Objects automatically align to surface normals
- **Bounds-Aware Placement**: Bottom of objects precisely placed on detected surfaces
- **Camera-Relative Rotation**: Objects face the user upon placement

#### 3. **Interactive Object Manipulation**
- Touch-based selection with visual indicators
- Scale, rotate, and translate using XR Interaction Toolkit
- Configurable min/max scale constraints
- Physics simulation with optional gravity
- Object deletion with confirmation UI

#### 4. **Screenshot Functionality**
- High-quality screenshot capture
- Smooth flash animation effect
- Preview display with fade animations
- Optimized texture handling

### 🌐 Backend & Cloud Integration

#### 1. **Advanced Download System**
```csharp
// Intelligent caching with progress tracking
public static async Task<DownloadResponse> RetrieveOrDownloadAsync(string url, 
    Action fetchCompletedCallback = null, 
    Action<float> progressCallback = null)
```

**Features**:
- ✅ **Smart Caching**: Hash-based file identification
- ✅ **Resume Support**: Checks existing file sizes before re-downloading
- ✅ **Progress Tracking**: Real-time download progress with callbacks
- ✅ **Optimized Buffering**: Dynamic buffer sizing (64KB - 1MB) based on file size
- ✅ **Async I/O**: Non-blocking file operations with `useAsync: true`
- ✅ **Error Handling**: Comprehensive exception handling with cleanup
- ✅ **Performance**: 1MB file buffer for maximum I/O throughput

#### 2. **GLTF Asset Loading**
```csharp
// Optimized GLTF loading with frame-budget management
public static async Task<GLTFAsset> LoadAsync(string filePath)
```

**Features**:
- ✅ **GLTFast Integration**: High-performance GLTF/GLB loading
- ✅ **Deferred Loading**: Frame-budget management to prevent frame drops
- ✅ **Resource Management**: Proper disposal and cleanup
- ✅ **Error Recovery**: Graceful failure handling

### 🎨 UI/UX Excellence

#### 1. **Animated Placement Wheel**
- **Radial Layout**: Mathematically calculated positioning
  ```csharp
  float x = radialDistance * Mathf.Sin(angleInRadians);
  float y = radialDistance * Mathf.Cos(angleInRadians);
  ```
- **Smooth Animations**: DOTween-powered transitions
- **Visual States**: Default, Loading, Cached, Error
- **Progress Indicators**: Radial fill progress bars
- **Responsive Design**: Adapts to different screen sizes

#### 2. **Selection System**
- Dynamic selection indicator that scales to object bounds
- Smooth positioning and rotation alignment
- Visual feedback for user interactions

#### 3. **Debug Menu Integration**
- AR plane visualization toggle
- Debug information display
- Adaptive positioning for different screen sizes
- Responsive layout for small devices (< 5 inches)

---

## 🏗️ Architecture & Design Patterns

### Design Patterns Implemented

#### 1. **Singleton Pattern**
```csharp
// Generic singleton for MonoBehaviours
public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour

// Generic singleton for ScriptableObjects
public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject

// Persistent singleton (survives scene loads)
public abstract class PersistentSingletonBehaviour<T> : SingletonBehaviour<T>
```

**Benefits**:
- Centralized access to managers and controllers
- Prevents duplicate instances
- Lazy initialization with automatic discovery

#### 2. **MVC-Inspired Architecture**

**Data Layer**:
- `PlaceableObject`: Object definitions and properties
- `ObjectPlacementPoint`: Placement data structures
- `CameraState`: Camera pose information
- `DownloadResponse`: Network operation results

**Controllers**:
- `ObjectPlacementController`: Main placement logic
- `ObjectPlacementUIController`: UI state management
- `ObjectSelectionController`: Selection handling
- `ScreenshotController`: Screenshot functionality

**Managers**:
- `PlaceableObjectManager`: Central object configuration
- `DownloadCache`: Download and caching system

**Components**:
- `ObjectPlacementWheel`: Wheel UI logic
- `ObjectPlacementWheelSlot`: Individual slot behavior
- `ObjectPlacementWheelIcon`: Icon positioning and display

#### 3. **Async/Await Pattern**
```csharp
// Non-blocking operations throughout
public async Task ShowAsync()
public async Task<DownloadResponse> DownloadAsync()
public async Task<GLTFAsset> LoadAsync(string filePath)
```

**Benefits**:
- Smooth UI interactions
- No frame drops during downloads
- Responsive user experience

#### 4. **Resource Reference Pattern**
```csharp
public struct ResourceReference<T> where T : Object
```

**Benefits**:
- Type-safe resource loading
- Automatic caching
- Editor-friendly with validation

### Code Organization

```
Phygtl.ARAssessment
├── Components/          # UI components and behaviors
├── Controllers/         # Application logic controllers
├── Core/               # Core systems and base classes
├── Data/               # Data structures and models
├── IO/                 # Input/Output and networking
└── Managers/           # Singleton managers
```

---

## 🔧 Technical Implementation

### 1. Object Placement Pipeline

```
User Tap → AR Raycast → Plane Detection → UI Wheel Display
    ↓
Object Selection → Download (if needed) → GLTF Load → Instantiate
    ↓
Add Components (Colliders, Rigidbody, ARTransformer) → Position & Rotate → Activate
```

**Key Code Snippets**:

```csharp
// Precise bottom-aligned placement
var bounds = Utility.GetObjectBounds(gameObject);
float bottomOffset = gameObject.transform.position.y - bounds.min.y;
var position = placement.raycastHit.pose.position + bottomOffset * placement.raycastHit.pose.up;
```

```csharp
// Camera-facing rotation
var direction = placement.cameraPose.position - placement.raycastHit.pose.position;
direction -= Vector3.Scale(direction, placement.raycastHit.pose.up);
direction.Normalize();
var rotation = Quaternion.LookRotation(direction, placement.raycastHit.pose.up);
```

### 2. Download & Caching System

**Architecture**:
```
URL Request → Hash Generation → Cache Check → Download (if needed) → File Write → Return Path
```

**Optimizations**:
- Dynamic buffer sizing based on file size
- Async file I/O with 1MB buffer
- Resume capability for interrupted downloads
- Progress callbacks for UI updates

```csharp
// Optimal buffer calculation
private static int CalculateOptimalBufferSize(long totalBytes)
{
    if (totalBytes < 1_048_576) // < 1 MB
        return Mathf.Clamp((int)(totalBytes / 8), 65536, 131072);
    else if (totalBytes < 10_485_760) // < 10 MB
        return Mathf.Clamp((int)(totalBytes / 32), 131072, 262144);
    else if (totalBytes < 52_428_800) // < 50 MB
        return Mathf.Clamp((int)(totalBytes / 128), 262144, 524288);
    else // >= 50 MB
        return 1048576; // 1 MB for maximum speed
}
```

### 3. UI Animation System

**DOTween Integration**:
```csharp
// Smooth wheel reveal
await image.DOFillAmount(1f, animationDuration).AsyncWaitForCompletion();

// Parallel slot animations
await Task.WhenAll(slots.Select(slot => slot.ShowAsync()));

// Flash effect
yield return flashImage.DOFade(0f, flashDuration).WaitForCompletion();
```

### 4. Collision & Physics Setup

```csharp
// Automatic mesh collider generation
foreach (var filter in gameObject.GetComponentsInChildren<MeshFilter>(true))
{
    if (!filter.TryGetComponent<Collider>(out _))
    {
        var meshCollider = filter.gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = filter.mesh;
        meshCollider.convex = true;
    }
}

// Physics configuration
rigidbody.isKinematic = !manager.useGravity;
rigidbody.useGravity = manager.useGravity;
rigidbody.mass = placeableObject.mass;
```

---

## 📋 Prerequisites

### Software Requirements
- **Unity**: 6000.0.58f2 or later
- **IDE**: Visual Studio 2022 or JetBrains Rider
- **Platform SDK**: 
  - Android SDK (API 24+) for Android builds
  - Xcode (latest) for iOS builds

### Hardware Requirements
- **Device**: ARCore (Android) or ARKit (iOS) compatible device
- **Camera**: Rear-facing camera with AR support
- **OS**: 
  - Android 7.0 (API 24) or higher
  - iOS 12.0 or higher

### Development Environment
- **Windows 10/11** or **macOS** for Unity development
- **Git** for version control
- **XAMPP** or similar local server for testing with local models

---

## 🚀 Setup & Installation

### 1. Clone the Repository
```bash
git clone <repository-url>
cd "Phygtl AR Assessment"
```

### 2. Open in Unity
1. Launch **Unity Hub**
2. Click **"Add"** → Select project folder
3. Ensure **Unity 6000.0.58f2** or later is installed
4. Click **"Open"**

### 3. Package Installation
Unity will automatically resolve and install dependencies:
- AR Foundation 6.0.6
- XR Interaction Toolkit 3.0.8
- GLTFast 6.14.1
- Universal Render Pipeline 17.0.4
- Input System 1.14.2
- DOTween (included in Plugins)

### 4. Local Server Setup (Optional)

To test with local models:

1. **Install XAMPP** (or Apache/nginx)
2. **Copy Models**: Place contents of `Models/` folder to server directory
3. **Configure URLs**: 
   - In Unity Editor, open `Assets/Resources/PlaceableObjectManager.asset`
   - Enable **"Use Local Download"**
   - Set **"Local Download URL"** for each object to your local server URL
   - Example: `http://localhost/Models/BoomBox.glb`

**Available Models** (in `Models/` folder):
- `BoomBox.glb`
- `ChessGame.glb`
- `SheenChair.glb`
- `ToyCar.glb`
- `WaterBottle.glb`

### 5. Scene Setup
- Main scene: `Assets/Scenes/SampleScene.unity`
- Press **Play** in Unity Editor to test
- For device testing, build and deploy to AR-compatible device

---

## 🎮 Usage Guide

### Running the Application

1. **Launch**: Open the app on your AR device
2. **Environment Scan**: Move device to scan for horizontal surfaces
3. **Plane Detection**: Semi-transparent planes appear on detected surfaces
4. **Place Objects**: Tap on a plane to open the placement wheel

### Placement Wheel Interface

**Wheel Elements**:
- **Circular Segments**: Each represents a placeable object
- **Icons**: Visual representation of each object
- **Progress Ring**: Shows download/load status
  - **Cyan**: Downloading
  - **Green**: Cached (ready)
  - **Red**: Error

**Status Indicators**:
- **Loading Animation**: Pulsing ring while fetching
- **Progress Fill**: Radial fill during download
- **Color Feedback**: Visual state indication

### Object Interaction

**Gestures**:
- **Single Tap**: Select object (shows selection indicator)
- **Drag**: Move object on detected plane
- **Pinch**: Scale object (within configured limits)
- **Two-Finger Rotate**: Rotate object around vertical axis

**UI Buttons**:
- **Create Button** (bottom): Opens placement wheel
- **Delete Button**: Removes selected object
- **Screenshot Button**: Captures current AR view
- **Options Menu**: Access debug settings

### Debug Features

**AR Debug Menu**:
- Toggle plane visualization
- View AR session information
- Display camera configurations
- Show/hide debug overlays

**Accessing Debug Menu**:
1. Tap **Options** button
2. Enable **"Show Debug Menu"**
3. Access debug controls from bottom toolbar

---

## 📁 Project Structure

```
Assets/
├── Scenes/
│   └── SampleScene.unity              # Main AR scene
│
├── Scripts/
│   └── Runtime/
│       ├── Components/                # UI & interaction components
│       │   ├── ObjectPlacementWheel.cs
│       │   ├── ObjectPlacementWheelSlot.cs
│       │   └── ObjectPlacementWheelIcon.cs
│       │
│       ├── Controllers/               # Application controllers
│       │   ├── ObjectPlacementController.cs      # Main placement logic
│       │   ├── ObjectPlacementUIController.cs    # UI management
│       │   ├── ObjectSelectionController.cs      # Selection handling
│       │   └── ScreenshotController.cs           # Screenshot feature
│       │
│       ├── Core/                      # Core systems & utilities
│       │   ├── SingletonBehaviour.cs             # Singleton pattern
│       │   ├── ScriptableSingleton.cs            # ScriptableObject singleton
│       │   ├── PersistentSingletonBehaviour.cs   # Persistent singleton
│       │   ├── ResourceReference.cs              # Type-safe resource loading
│       │   ├── GLTFAsset.cs                      # GLTF loading wrapper
│       │   └── GLTFHelper.cs                     # GLTF utilities
│       │
│       ├── Data/                      # Data structures
│       │   ├── PlaceableObject.cs                # Object definition
│       │   ├── ObjectPlacementPoint.cs           # Placement data
│       │   └── CameraState.cs                    # Camera pose
│       │
│       ├── IO/                        # Input/Output & networking
│       │   ├── DownloadCache.cs                  # Download & caching system
│       │   └── DownloadResponse.cs               # Download result
│       │
│       ├── Managers/                  # Singleton managers
│       │   └── PlaceableObjectManager.cs         # Object configuration
│       │
│       └── AppDebugger.cs             # Custom logging system
│
├── Prefabs/                           # Reusable prefabs
│   ├── PlaceableObject.prefab                    # Object container
│   ├── PlacementWheel.prefab                     # Wheel UI
│   ├── PlacementWheelSlot.prefab                 # Wheel slot
│   └── PlacementWheelIcon.prefab                 # Wheel icon
│
├── Resources/                         # Runtime resources
│   ├── PlaceableObjectManager.asset              # Configuration
│   ├── DOTweenSettings.asset                     # Animation settings
│   └── Sprites/                                  # UI sprites
│
├── Settings/                          # Render pipeline settings
│   ├── Mobile_RPAsset.asset                      # Mobile URP settings
│   ├── PC_RPAsset.asset                          # PC URP settings
│   └── UniversalRenderPipelineGlobalSettings.asset
│
├── UI/                                # UI assets
│   ├── camera_icon.png
│   ├── circle_border.png
│   ├── circle_inner.png
│   └── circle_outer.png
│
├── XR/                                # XR configuration
│   ├── Settings/                                 # XR settings per platform
│   └── XRGeneralSettingsPerBuildTarget.asset
│
├── MobileARTemplateAssets/            # AR template assets
├── Plugins/                           # Third-party plugins
│   ├── Demigiant/                                # DOTween
│   └── ARFoundationRemoteInstaller/              # Remote debugging
│
└── Samples/                           # XR Interaction Toolkit samples

Models/                                # GLTF models for placement
├── BoomBox.glb
├── ChessGame.glb
├── SheenChair.glb
├── ToyCar.glb
└── WaterBottle.glb
```

---

## 💎 Code Quality & Best Practices

### Documentation Standards

**Every class, method, and property is documented**:
```csharp
/// <summary>
/// A controller for placing objects in the scene.
/// It uses the Enhanced Touch API to detect finger down events and raycast 
/// to the plane to find the placement point.
/// </summary>
[RequireComponent(typeof(ARRaycastManager), typeof(ObjectPlacementUIController))]
public class ObjectPlacementController : SingletonBehaviour<ObjectPlacementController>
```

### Code Organization

✅ **Regions for clarity**:
```csharp
#region Properties
#region Fields
#region Methods
#region Helper Methods
#region Event Handlers
```

✅ **Namespace organization**:
```csharp
#region Namespaces
using System;
using UnityEngine;
// ...
#endregion
```

### Error Handling

**Comprehensive validation**:
```csharp
if (!manager)
{
    AppDebugger.LogError("Couldn't initialize because PlaceableObjectManager not found!", 
        this, nameof(ObjectPlacementWheel));
    return;
}
```

**Custom logging system**:
```csharp
public static class AppDebugger
{
    [HideInCallstack]
    public static void Log(string message, Object context = null, string sourceName = "App")
    
    [HideInCallstack]
    public static void LogWarning(string message, Object context = null, string sourceName = "App")
    
    [HideInCallstack]
    public static void LogError(string message, Object context = null, string sourceName = "App")
}
```

### Memory Management

✅ **Proper disposal**:
```csharp
public void Dispose()
{
    importer?.Dispose();
    importer = null;
}
```

✅ **Resource cleanup**:
```csharp
internal void Clear()
{
    downloadTask = null;
    gltfAsset?.Dispose();
    gltfAsset = null;
}
```

### SOLID Principles

- **Single Responsibility**: Each class has one clear purpose
- **Open/Closed**: Extensible through inheritance (Singleton base classes)
- **Liskov Substitution**: Proper use of generics and inheritance
- **Interface Segregation**: Focused interfaces (IPointerClickHandler)
- **Dependency Inversion**: Dependency on abstractions (managers, controllers)

---

## ⚡ Performance Optimizations

### 1. Download System
- **Dynamic Buffering**: 64KB to 1MB based on file size
- **Async I/O**: Non-blocking file operations
- **Smart Caching**: Prevents redundant downloads
- **Resume Support**: Validates existing files before re-downloading

### 2. GLTF Loading
- **Frame Budget Management**: `TimeBudgetPerFrameDeferAgent` prevents frame drops
- **Deferred Loading**: Spreads work across multiple frames
- **Resource Pooling**: Cached GLTF assets for reuse

### 3. UI Rendering
- **Raycast Optimization**: Selective raycast targets
- **Batch Animations**: Parallel async operations with `Task.WhenAll`
- **Efficient Layout**: Mathematical calculations instead of layout groups

### 4. Physics
- **Convex Colliders**: Optimized collision detection
- **Kinematic Mode**: Optional physics for better performance
- **Selective Rigidbodies**: Only on placed objects

### 5. Memory
- **Object Disposal**: Proper cleanup of GLTF assets
- **Texture Management**: Efficient sprite handling
- **Cache Management**: Hash-based file identification

---

## 🔨 Build Instructions

### Android Build

#### 1. Configure Player Settings
```
File → Build Settings → Android
```

**Player Settings**:
- **Company Name**: Phygtl
- **Product Name**: AR Assessment Yassine
- **Package Name**: `com.phygtl.arassessment.yassine`
- **Minimum API Level**: 24 (Android 7.0)
- **Target API Level**: Latest
- **Scripting Backend**: IL2CPP
- **Target Architectures**: ARM64

**XR Settings**:
- ✅ ARCore Support enabled
- ✅ AR Foundation configured

#### 2. Build Process
```
1. File → Build Settings → Android
2. Click "Build" or "Build and Run"
3. Choose output location
4. Wait for build completion
```

#### 3. Deploy to Device
```bash
# Via ADB
adb install -r YourApp.apk

# Or use "Build and Run" in Unity
```

### iOS Build

#### 1. Configure Player Settings
```
File → Build Settings → iOS
```

**Player Settings**:
- **Bundle Identifier**: `com.phygtl.arassessment.yassine`
- **Target iOS Version**: 12.0 or higher
- **Architecture**: ARM64
- **Camera Usage Description**: "This app uses the camera for AR experiences"

**XR Settings**:
- ✅ ARKit Support enabled
- ✅ AR Foundation configured

#### 2. Build Process
```
1. File → Build Settings → iOS
2. Click "Build"
3. Choose output location
4. Open generated Xcode project
```

#### 3. Xcode Configuration
```
1. Open .xcodeproj file
2. Select your development team
3. Configure signing & capabilities
4. Add "Camera" to capabilities if needed
5. Build and deploy to device
```

### Editor Testing

**XR Simulation**:
- Unity 6 includes XR simulation for testing without device
- Use included simulation environments
- Test plane detection and object placement in editor

**AR Foundation Remote** (included):
- Test on device while running in editor
- Real-time debugging on actual hardware
- See `Assets/Plugins/ARFoundationRemoteInstaller/`

---

## 🎓 Skills Demonstrated

### ✅ AR Development
- AR Foundation integration
- Plane detection and tracking
- Spatial understanding and object placement
- XR Interaction Toolkit usage

### ✅ UI/UX Development
- Custom UI components
- Animation systems (DOTween)
- Responsive design
- Visual feedback and polish

### ✅ Backend Development
- Async/await patterns
- HTTP networking
- File I/O operations
- Caching strategies

### ✅ Cloud Connectivity
- Remote asset loading
- Progress tracking
- Error handling
- Resume capability

### ✅ Software Architecture
- Design patterns (Singleton, MVC)
- SOLID principles
- Clean code practices
- Comprehensive documentation

### ✅ Performance Optimization
- Memory management
- Frame budget management
- Efficient algorithms
- Resource pooling

### ✅ Production Quality
- Error handling
- Logging system
- Code organization
- Maintainability

---

## 📞 Contact

**Developer**: Yassine  
**Project Type**: Upwork Skill Assessment  
**Status**: ✅ Complete and Ready for Review

---

## 📄 License

This project is developed as a skill assessment for client evaluation purposes.

---

**Built with ❤️ by Yassine**
