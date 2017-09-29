/////////////////////////////////////////////////////////////////////////////////////////////////
// Authors: Fran Molero                fmolero@cvc.uab.es
//          Marc Puig                  mgarcia@cvc.uab.es
//          Jose A. Iglesias-Guitian    jalley@cvc.uab.es
//
// Date: September 2017
//
/////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/* 
 * For now this script under development saves the output of the beauty pass of Octane Renderer and some infor passes  (e.g. depth, semantic segmentation).
 * Main idea: Unity waits until Octane Renderer is ready to dump a rendered frame, meanwhile Unity simulations are frozen using Time.timeScale = 0; 
*/

[RequireComponent(typeof(Camera))]

public class RenderPass
{
    public Octane.RenderPassId _renderPass;
    public Octane.ImageSaveType _renderPassOutputType;
    public bool _capturePass;
    public string _outputDir;

    public RenderPass(Octane.RenderPassId renderPass, Octane.ImageSaveType renderPassOutputType, bool capturePass = true, string outputDir = "/../Output/Octane")
    {
        _renderPass = renderPass;
        _renderPassOutputType = renderPassOutputType;
        _capturePass = capturePass;
        _outputDir = Application.dataPath + outputDir;
    }
}

public class SaveOctaneFrameSequence : MonoBehaviour
{
    //private VZSemanticSegmentation semantic;

    private Octane.RenderPassId[] renderPass;
    //public Octane.ImageSaveType[] renderPassOutputType;
    //public bool[] capturePass;

    public List<RenderPass> my_render_passes;
    public RenderPass[] my_render_passes_array;

    //public tOctaneCaptureLayers m_OctaneCaptureLayers;

    public int m_CaptureFramerate = 30;
    public int m_VSynchCount = 0;
    public uint m_MaxSamplesPerPixel = 0; // this will automatically take later its value from Octane Kernel configuration
    public bool m_CaptureOctane = true;
    //public bool m_CaptureUnity = false;
    //public bool m_CaptureSemantic = false;

    //private string mOctaneOutputDir;
    //private string mUnityOutputDir;
    //private string mSemanticOutputDir;
    private bool mCapturedFrame;
    private uint m_OctaneSavedFrames = 0;

    private uint mPreviousFrameOctaneSpp = 0;

    // Use this for initialization
    void Start()
    {
        // Create output directories
        //mOctaneOutputDir = Application.dataPath + "/../Output/Octane";
        //Directory.CreateDirectory(mOctaneOutputDir);
       // mUnityOutputDir = Application.dataPath + "/../Output/Unity";
        //Directory.CreateDirectory(mUnityOutputDir);
        //mSemanticOutputDir = Application.dataPath + "/../Output/SemanticSegmentation";
        //Directory.CreateDirectory(mSemanticOutputDir);

        m_MaxSamplesPerPixel = Octane.Renderer.GetLatestRenderStatistics().maxSamplesPerPixel;
        // Configure framerate specifications
        QualitySettings.vSyncCount = m_VSynchCount;
        Time.captureFramerate = m_CaptureFramerate;
        Application.runInBackground = true;
        //Application.targetFrameRate = 10;        
        mPreviousFrameOctaneSpp = 0;
        m_OctaneSavedFrames = 0;
        mCapturedFrame = false;

        my_render_passes = new List<RenderPass>();
        my_render_passes.Add(new RenderPass(Octane.RenderPassId.RENDER_PASS_BEAUTY, 
                                            Octane.ImageSaveType.IMAGE_SAVE_TYPE_EXR, 
                                            true, 
                                            "/../Output/Octane/Beauty"));
        my_render_passes.Add(new RenderPass(Octane.RenderPassId.RENDER_PASS_Z_DEPTH, 
                                            Octane.ImageSaveType.IMAGE_SAVE_TYPE_EXR, 
                                            true, 
                                            "/../Output/Octane/Depth"));
        my_render_passes.Add(new RenderPass(Octane.RenderPassId.RENDER_PASS_RENDER_LAYER_ID, 
                                            Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG8, 
                                            true, 
                                            "/../Output/Octane/SemanticSegmentation"));
        // Add new render passes here ...

        //renderPass.Initialize();
        //renderPass = new Octane.RenderPassId[Constants.RenderPasses];
        //renderPassOutputType = new Octane.ImageSaveType[Constants.RenderPasses];
        //capturePass = new bool[Constants.RenderPasses];
        //renderPass[0] = Octane.RenderPassId.RENDER_PASS_BEAUTY;
        //renderPass[1] = Octane.RenderPassId.RENDER_PASS_Z_DEPTH;
        //renderPass[2] = Octane.RenderPassId.RENDER_PASS_RENDER_LAYER_ID;
        //renderPassOutputType[0] = Octane.ImageSaveType.IMAGE_SAVE_TYPE_EXR;
        //renderPassOutputType[1] = Octane.ImageSaveType.IMAGE_SAVE_TYPE_EXR;
        //renderPassOutputType[2] = Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG8;
        //Octane.Renderer.SetRenderPasses(renderPass);
        //capturePass[0] = true;
        //capturePass[1] = true;
        //capturePass[2] = true;

        // list to array
        my_render_passes_array = my_render_passes.ToArray();
        int NumPasses = my_render_passes_array.GetLength(0);
        renderPass = new Octane.RenderPassId[NumPasses];
        //renderPassOutputType = new Octane.ImageSaveType[NumPasses];
        //capturePass = new bool[NumPasses];

        for (uint i = 0; i < NumPasses; ++i)
        {
            renderPass[i] = my_render_passes_array[i]._renderPass;
            //renderPassOutputType[i] = my_render_passes_array[i]._renderPassOutputType;
            //capturePass[i] = my_render_passes_array[i]._capturePass;
            Directory.CreateDirectory(my_render_passes_array[i]._outputDir);
        }        
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
                for (int pass = 0; (pass < my_render_passes_array.GetLength(0)) && my_render_passes_array[pass]._capturePass; pass++)
                {
                    string frame_str = string.Format("{0}/octane_fr{1:D04}_pass{2}_spp{3}.png", my_render_passes_array[pass]._outputDir, m_OctaneSavedFrames, pass, Octane.Renderer.SampleCount);
                    Octane.Renderer.SaveImage(/*Octane.RenderPassId.RENDER_PASS_BEAUTY*/renderPass[pass],
                                          frame_str,
                                          /*Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG8*/ my_render_passes_array[pass]._renderPassOutputType,
                                          true/*asynchronous*/);
                }
                // save semantic segmentation (double check it exists)
                //if (m_CaptureSemantic)
                //{
                    //GameObject semantic_obj = GameObject.Find("GTCamera");
                    //Camera semantic_cam = semantic_obj.GetComponent<Camera>();
                    //semantic = semantic_obj.GetComponent<VZSemanticSegmentation>();
                    //semantic.SaveCameraToDisk(semantic_cam, mSemanticOutputDir);
                //}

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

        //if (m_CaptureUnity)
        //{
            // Capture Unity Render
            // To do: Append filename to folder name (format is '0005 shot.png"')
            //string name = string.Format("{0}/{1:D04} shot.png", folder, Time.frameCount);
            //string name_str = "unity_fr_" + Time.frameCount.ToString();
            // Capture the screenshot to the specified file.
            //ScreenCapture.CaptureScreenshot(mUnityOutputDir + "/" + name_str + ".png");
        //}
    }
}