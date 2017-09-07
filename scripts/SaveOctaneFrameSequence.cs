// Authors: Fran Molero fmolero@cvc.uab.es
//          Jose A. Iglesias-Guitian jalley@cvc.uab.es
// Date: September 2017
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/* For now this script under development saves the output of both Octane Renderer and Unity Editor (just for debugging purposes)
   Main idea: Unity waits until Octane Renderer is ready to dump a rendered frame, meanwhile Unity simulations are frozen using 
   Time.timeScale = 0; Observed behavior: when increasing m_CaptureFramerate through UI, it can be observed that Octane output 
   is smoothed.
*/
// TO DO: The desired behavior would be to ensure the time duration of a certain captured sequence and ideally chosing how 
// many Octane frames per second we want.

public class SaveOctaneFrameSequence : MonoBehaviour
{
    public Octane.RenderPassId[] renderPass;
    public Octane.ImageSaveType[] renderPassOutputType;

    //public tOctaneCaptureLayers m_OctaneCaptureLayers;

    public int m_CaptureFramerate = 30;
    public int m_VSynchCount = 0;
    public uint m_MaxSamplesPerPixel = 0; // this will automatically take later its value from Octane Kernel configuration
    public bool m_CaptureOctane = true;
    public bool m_CaptureUnity = false;

    private string mOctaneOutputDir;
    private string mUnityOutputDir;
    private bool   mCapturedFrame;
    private uint m_OctaneSavedFrames = 0;

    private uint mPreviousFrameOctaneSpp = 0;

    // Use this for initialization
    void Start()
    {
        // Create output directories
        mOctaneOutputDir = Application.dataPath + "/../Output/Octane";
        Directory.CreateDirectory(mOctaneOutputDir);
        mUnityOutputDir = Application.dataPath + "/../Output/Unity";        
        Directory.CreateDirectory(mUnityOutputDir);

        m_MaxSamplesPerPixel = Octane.Renderer.GetLatestRenderStatistics().maxSamplesPerPixel;
        // Configure framerate specifications
        QualitySettings.vSyncCount = m_VSynchCount;
        Time.captureFramerate = m_CaptureFramerate;
        Application.runInBackground = true;
        //Application.targetFrameRate = 10;        
        mPreviousFrameOctaneSpp = 0;
        m_OctaneSavedFrames = 0;
        mCapturedFrame = false;
        //renderPass.Initialize();
        renderPass = new Octane.RenderPassId[2];
        renderPassOutputType = new Octane.ImageSaveType[2];
        renderPass[0] = Octane.RenderPassId.RENDER_PASS_BEAUTY;
        renderPass[1] = Octane.RenderPassId.RENDER_PASS_Z_DEPTH;
        renderPassOutputType[0] = Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG8;
        renderPassOutputType[1] = Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG16;
        Octane.Renderer.SetRenderPasses(renderPass);
    }

    // Use this for initialization
    bool JustFinishedOctaneFrame()
    {
        return (
            (mCapturedFrame == false) && /* frame was not just written to disk before */
            !Octane.Renderer.IsCompiling && /* Octane should not be currently compiling the current scene frame */
            (mPreviousFrameOctaneSpp != Octane.Renderer.SampleCount) && /* we use this trick to avoid multiple images to be written with the same frame */
            Octane.Renderer.SampleCount.Equals(Octane.Renderer.GetLatestRenderStatistics().maxSamplesPerPixel) /* ensure desired spp has been achieved */
            );
    }

    void Update()
    {
        if (m_CaptureOctane)
        {
            if (JustFinishedOctaneFrame())
            {
                m_OctaneSavedFrames++;
                //string frame_str = "octane_fr_" + m_OctaneSavedFrames/*+ "_realfr_" + Time.frameCount.ToString()*/;
                for (int pass = 0; pass < renderPass.GetLength(0); pass++)
                {
                    string frame_str = string.Format("{0}/octane_fr{1:D04}_pass{2}_spp{3}.png", mOctaneOutputDir, m_OctaneSavedFrames, pass, Octane.Renderer.SampleCount);
                    Octane.Renderer.SaveImage(/*Octane.RenderPassId.RENDER_PASS_BEAUTY*/renderPass[pass],
                                          frame_str,
                                          /*Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG8*/ renderPassOutputType[1],
                                          true/*asynchronous*/);
                }
                //Octane.Renderer.SaveImage(Octane.RenderPassId.RENDER_PASS_AMBIENT_OCCLUSION,
                //                          mOctaneOutputDir + "/" + frame_str + "_AO_" + Octane.Renderer.SampleCount + "spp",
                //                          Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG16,
                //                          false/*synchronous*/);
                //Octane.Renderer.SaveImage(Octane.RenderPassId.RENDER_PASS_Z_DEPTH,
                //                          mOctaneOutputDir + "/" + frame_str + "_depth_" + Octane.Renderer.SampleCount + "spp",
                //                          Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG16,
                //                          false/*synchronous*/);
                print("Writing Octane image " + m_OctaneSavedFrames + " rendered with " + Octane.Renderer.GetLatestRenderStatistics().samplesPerPixel + "spp.");
                Time.captureFramerate = m_CaptureFramerate;
                Time.timeScale = 1;
                mCapturedFrame = true;
            }
            else
            {
                Time.timeScale = 0;
                mCapturedFrame = false;
            }
            mPreviousFrameOctaneSpp = Octane.Renderer.SampleCount;
        }

        if (m_CaptureUnity)
        {
            // Capture Unity Render
            // To do: Append filename to folder name (format is '0005 shot.png"')
            //string name = string.Format("{0}/{1:D04} shot.png", folder, Time.frameCount);
            string name_str = "unity_fr_" + Time.frameCount.ToString();
            // Capture the screenshot to the specified file.
            ScreenCapture.CaptureScreenshot(mUnityOutputDir + "/" + name_str + ".png");
        }
    }
}