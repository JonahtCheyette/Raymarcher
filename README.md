# Raymarcher
This is a renderer I made in unity using a technique called raymarching. Not to be confused with raytracing, though the techniques are similar. The way ay rayTRACER works is that for each pixel on screen you have a ray, defined by a direction and a start point. You then calculate where each ray intersects with scene geometry, and then based on what it intersected with, you calculate whether to make the ray refract or reflect or whatever else you want to do. The way a rayMARCHER, on the other hand, works is slightly different. It starts the same way a raytracer does, with a ray for each pixel. The difference is that instead of simply calculating where the ray intersects with scene geometry, you march along the direction of the ray, until you hit the scene geometry. The technique I used in particular is called sphere tracing, where you get the distance to the scene geometry, and that's the distance you march along the ray, because that's the furthest distance you can go while guaranteed to not hit something. An intersection is simply where the distance to the scene is below a certain threshold. I'm glazing over a lot of things here, but the benefits of raymarching are that you can do things like outlines and smooth blending of shapes at no extra rendering cost, and you can render any shape you can define a signed distance function for (a good list of those is found [here](https://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm). Other articles on this same website also go more in depth about what you can do with raymarching). The downsides are that, even when compared to raytracing, it's computationally intensive. My implementation doesn't do anything like reflections or refractions, so it's not too bad, but even so it's not great.

## How To Set Up The Project
This section is for those who haven't downloaded a unity project over github before. For those who have, this project was created in Unity 2019.4.3f1, so if it isn't working on your install try opening it up on that version.  
Firstly, you're gonna need unity, specifically version 2019.4.3f1. There's a free personal version of unity, so download that, then go to installs in the unity hub, hit add, and select Unity 2019.4.3f1. After that clone this repository to your computer, then open up unity hub, go to **Projects>Add** and navigate to the cloned repository.

## How to actually use the project
### Setting up the scene
Go to the folder marked **Prefabs**. Inside are unity prefabs markes shpere, torus, etc. These shapes will be how you set up the scene. Simply drag them into the scene, and alter their positions, rotations, scale, etc to get them as you like. In addition, some of the prefabs will have settings you can alter. In the inspector, look for the _____ Data Passer Script component attatched to the given prefab, and there will be a selection of parameters you can alter, depending on the prefab. The objects will alter themselves in the scene view. One thing to note is that the scale in the transform of the prefab only affects the cube prefab. All the others have custom ways to set their size.  
  
***All Objects***  
**Color:** Self Explanatory. The color of the object. Note that changing materials won't actually change the shape's appearance once you start rendering. The only thing that alters their appearance is this parameter.  
  
***Capsule/Cylinder***  
**Height:** The height of the shape.  
**Radius:** The radius of the cylinder. In the case of the capsule, also changes the radius of the hemispheres that end the capsule.  
  
***Plane***  
**Distance Along Normal:** All planes are designated by a normal vector and a distance along said normal. In my implementaion, the renderers grab the normal vector from the plane's orientation in space. All that's left then is the distance along said normal the plane rests. Can also be thought of as the minimum distance from the plane to the origin.  
  
***Sphere***  
**Radius:** The radius of the sphere.  
  
***Torus***  
**Ring Radius:** The radius of the torus' "hoop"  
**Outer Circle Radius:** The radius of the edge of the torus.  
  
### Rendering
Once you've got the scene set up how you like it, go to the main camera object, and activate whichever renderer you'd like, then hit play. Below find a list of all the different raymarchers, and the parameters you can set in the raymarchers themselves. Note that only the lowest raymarcher activated will be drawn to the scene, so it's pointless to have multiple activated at a time. In fact, all of the active raymarchers will render, however they're drawn to the scene in order, so only the last one renders. This means that having multiple active is just wasting processing power.  
  
***All Raymarchers***  
**Lighting:** The lighting to use for the scene. Use either a point light or a directional light. If you change the light, or change how it's positioned, the scene will update in real time.  
**Shader/Ray Marching Shader:** Holds a reference to the script the raymarcher uses for rendering. DON'T TOUCH!
**Auto Update:** Whether or not to update teh raymarcher automatically if you change part of the scene. In other words, if you move one of the shapes, and this is active, this will automatically update the render.
  
***Barebones Compute Shader Renderer***  
As its name suggests, this is the most barebones renderer in the project. Ignores the scene you've so lovingly set up to render something I've already hardcoded in. In this case, it's a weird 3d representation of the mandelbrot set, with a glowing outline. As such, no special parameters.  
  
***Base Ray Marching Master***  
The most basic ray marcher in the project. Simply takes your scene, and renders it using raymarching. As such, it's kind of pointless. I mostly made it to make sure I understood the theory, before moving on to cooler applications. No special parameters.  
  
***Intersection Ray Marching Master***  
Allows you to set a special intersection type for each object in the scene. Note that order matters, and the first shape should always be set to union. If you want to change the order, unfortunately at present the ony way is to add new shapes in, and delete old ones. The order is newest shapes first.  
**Union:** The regular rendering type.  
**Subtraction:** Makes the shape, as well as anything that is intersecting with it, invisible.  
**Intersection:** Makes it so that only things intersecting with the shape render.  
  
***Modifier Ray Marching Master***  
Allows you to set a series of modifiers for each shape.
**Elongation:** Stretches out the shape in the x, y and z directions. Leave as 0 to do nothing.  
**Rounding:** Rounds the shape. Note that this increases the shape's size.  
  
***Outline Ray Marching Master***  
Allows you to create an outline around the shapes. You can set a size and strength for the outline, as well as an inner and outer color. Note that outline strength should be kept relatively small, around 0.01-ish. One thing to try out is making all of your shapes black, and having a cool outline that way.  
  
***Smooth Ray Marching Master***  
Allows you to set a smoothing parameter that causes shapes to blend together. The higher the smoothing parameter, the more blended together the shapes.  
  
***Melt The Computer***  
All of the raymarchers above, put together. In addition, there are 2 new modifiers you can assign to each shape.  
**Onion:** Whether or not to "Onion" the shape, or have the shape be a hollow shell.  
**Onion Layer Thickness:** How thick the shell should be.

## Screenshots
![Image 1](https://cdn.discordapp.com/attachments/647518062328938497/911811172729253928/Screenshot_76.png)  
A composite shape, made using the intersection ray marcher. Made with a cube, a sphere, and 3 cylinders.  
  
![Image 2](https://cdn.discordapp.com/attachments/647518062328938497/911811173089951804/Screenshot_77.png)  
The same shape, but lit with a point light from the inside, and given an outline  
  
![Image 3](https://cdn.discordapp.com/attachments/647518062328938497/911811173370961980/Screenshot_78.png)  
My attempt at the Dr. Manhattan logo.  
  
![Image 4](https://cdn.discordapp.com/attachments/647518062328938497/911811173601652746/Screenshot_79.png)  
A bowl made using the onioning feature  
  
![Image 5](https://cdn.discordapp.com/attachments/647518062328938497/911811173836554311/Screenshot_80.png)  
A tower, made to show off the smoothing feature.  
