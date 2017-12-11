# octane4unity
Set of C# scripts to use Octane renderer under Unity

### Recorder Scripts ###

+ **Recorder.cs**: saves replay an animation for dynamic objects under the "DynamicObjects" node name in a given hierarchy.
+ **RecorderEditor.cs**: Editor side for the Recorder script. Not assigned in the scene.
+ **SaveOctaneFrameSequence.cs**: this script saves images of a REnderTarget camera viewport rendered with Octane.
+ **TimeScaleScript.cs**: it justs sets the Unity timeScale to 1 at the beginning. It may live in the camera node.

### Traffic Light Scripts ###
+ **TrafficLight.cs**: controls timing for traffic lights, it might contain one or more trafficLightController. Each traffic light (trigger) owns one of these scripts.
+ **TrafficLightController.cs**: modifies traffic lights according to each traffic light state. One for each traffic light.
+ **TrafficLightGroup.cs**: it manages the group of TrafficLight living as children. Each traffic lights group owns one script.
+ **TrafficLightPed.cs**: it manages pedestrian traffic lights. Each traffic light for pedestrians owns one.

### Car Scripts ###
+ **SynthiaCarAI.cs**: AI for cars. It needs to have a path setted.It uses a RayPoint to detect other cars. Each car owns one.
+ **CarController.cs**: it controls car physics. One for each car.
+ **WaypointTracker.cs**: Unity script to follow one path. Used by SynthiaCarAI (check with Xisco).

### Pedestrian Scripts ###
+ **PedestrianAI.cs**: AI for pedestrians, pedestrian follows a pedestrianPath until arriving to the end. One for each pedestrian.
+ **PedestrianPath.cs**: draws the path that pedestrian can follow. It is necessary that a path owns this script to make it possible for pedestrian to follow it. One for each path.
