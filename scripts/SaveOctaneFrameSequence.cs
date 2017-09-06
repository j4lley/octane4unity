using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveOctaneFrameSequence : MonoBehaviour
{
    public int m_CaptureFramerate = 30;
    public int m_VSynchCount = 0;
    public uint m_MaxSamplesPerPixel = 0; // this will automatically take later its value from Octane Kernel configuration
    public bool m_CaptureOctane = true;
    public bool m_CaptureUnity = true;

    private string mOctaneOutputDir;
    private string mUnityOutputDir;
    private bool   mCapturedFrame;
    private uint m_OctaneSavedFrames = 0;

    private bool condition;
    //private int frame;

    public IEnumerator Run() {
        yield return new WaitForSeconds(1f);
        print("::::::::::::: Bomb exploded ::::::::::: !!!!");
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Real Time (The real time in seconds since the game started): " + Time.realtimeSinceStartup);
        Debug.Log("Fixed Time (This is the time in seconds since the start of the game): " + Time.fixedTime);
        Debug.Log("Time: " + Time.time);
        Application.Quit();
    }

    // Use this for initialization
    void Start()
    {
        // Create output directories
        mOctaneOutputDir = Application.dataPath + "/../Output/Octane";
        Directory.CreateDirectory(mOctaneOutputDir);
        mUnityOutputDir = Application.dataPath + "/../Output/Unity";        
        Directory.CreateDirectory(mUnityOutputDir);

        m_OctaneSavedFrames = 0;
        mCapturedFrame = false;
        m_MaxSamplesPerPixel = Octane.Renderer.GetLatestRenderStatistics().maxSamplesPerPixel;
        // Configure framerate specifications
        QualitySettings.vSyncCount = m_VSynchCount;
        Time.captureFramerate = m_CaptureFramerate;
        //Application.targetFrameRate = 10;        

        // Tests
        //StartCoroutine(Run());
        //StartCoroutine(Example());
        condition = false;
        //frame = 0;
    }

    IEnumerator WaitForOctaneRender()
    {
        while (!Octane.Renderer.GetLatestRenderStatistics().samplesPerPixel.Equals(m_MaxSamplesPerPixel) /*&& Octane.Renderer.IsRendering*/ 
            || !Octane.Renderer.GetLatestRenderStatistics().state.Equals(Octane.RenderState.RSTATE_FINISHED))
        {
            //print("Waiting for Octane to finish render ...");
            mCapturedFrame = false;
            Time.timeScale = 0;
            Time.captureFramerate = -1;
            yield return 0;
        }

        if (!mCapturedFrame)
        {
            if (m_CaptureOctane)
            {
                m_OctaneSavedFrames++;
                string frame_str = "octane_fr_" + m_OctaneSavedFrames/*+ "_realfr_" + Time.frameCount.ToString()*/;
                Octane.Renderer.SaveImage(Octane.RenderPassId.RENDER_PASS_BEAUTY,
                                        mOctaneOutputDir + "/" + frame_str + "_" + Octane.Renderer.SampleCount + "spp",
                                        Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG8,
                                        false/*synchronous*/);
                //Octane.Renderer.SaveImage(Octane.RenderPassId.RENDER_PASS_AMBIENT_OCCLUSION,
                //                          mOctaneOutputDir + "/" + frame_str + "_AO_" + Octane.Renderer.SampleCount + "spp",
                //                          Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG16,
                //                          false/*synchronous*/);
                //Octane.Renderer.SaveImage(Octane.RenderPassId.RENDER_PASS_Z_DEPTH,
                //                          mOctaneOutputDir + "/" + frame_str + "_depth_" + Octane.Renderer.SampleCount + "spp",
                //                          Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG16,
                //                          false/*synchronous*/);
                print("Writing Octane image " + m_OctaneSavedFrames + " rendered with " + Octane.Renderer.GetLatestRenderStatistics().samplesPerPixel + "spp.");
            }
            mCapturedFrame = true;
            Time.captureFramerate = m_CaptureFramerate;
            Time.timeScale = 1;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    // EXAMPLE TEST
    /////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator Example()
    {
        Debug.Log("Waiting for prince/princess to rescue me...");
        Time.timeScale = 0;
        Time.captureFramerate = 0;
        yield return new WaitWhile(() => condition);

        Debug.Log("Finally I have been rescued!");
        string frame_str = "octframe_" + Time.frameCount.ToString();
        Octane.Renderer.SaveImage(Octane.RenderPassId.RENDER_PASS_BEAUTY,
                                 mOctaneOutputDir + "/" + frame_str + "_" + Octane.Renderer.SampleCount + "spp",
                                 Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG8,
                                 false/*synchronous*/); // TO DO : CHECK ASYNCH Vs SYNCH
        Time.timeScale = 1;
        Time.captureFramerate = 10;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(WaitForOctaneRender());
        if (m_CaptureUnity)
        {
            // Capture Unity Render
            // To do: Append filename to folder name (format is '0005 shot.png"')
            //string name = string.Format("{0}/{1:D04} shot.png", folder, Time.frameCount);
            string name_str = "unity_fr_" + Time.frameCount.ToString();
            // Capture the screenshot to the specified file.
            ScreenCapture.CaptureScreenshot(mUnityOutputDir + "/" + name_str + ".png");
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // TESTS
        /////////////////////////////////////////////////////////////////////////////////////////////
#if FALSE
        //        condition = ( (Octane.Renderer.GetLatestRenderStatistics().samplesPerPixel == mMaxSamplesPerPixel) &&
        (Octane.Renderer.GetLatestRenderStatistics().state == Octane.RenderState.RSTATE_FINISHED) );
        //frame++;

        //StartCoroutine(Example());
#endif

#if FALSE
        if (Octane.Renderer.SampleCount.Equals(mMaxSamplesPerPixel))
        {
            Debug.Log("Octane Renderer save image !!!");
            string frame_str = "frame_" + Time.frameCount.ToString();
            Octane.Renderer.SaveImage(Octane.RenderPassId.RENDER_PASS_BEAUTY,
                                  mOutputDir + "/" + frame_str + "_" + Octane.Renderer.SampleCount + "spp",
                                  Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG8,
                                  false/*synchronous*/);
            Time.timeScale = 1;
        }

        else if (Octane.Renderer.IsRendering || Octane.Renderer.HasFrameWaiting || Octane.Renderer.IsCompiling)
        {
            Debug.Log("Octane Renderer is busy ...");
            Time.timeScale = 0;
        }
#endif
    }
}