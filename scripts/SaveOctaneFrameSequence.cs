using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveOctaneFrameSequence : MonoBehaviour
{

    private string mOutputDir;
    private bool   mCapturedFrame;
    private uint   mMaxSamplesPerPixel;

    // Use this for initialization
    void Start()
    {
        mOutputDir = Application.dataPath + "/octane_output";
        Directory.CreateDirectory(mOutputDir);
        Debug.Log("Created directory Octane Capture Script!");
        mCapturedFrame = false;
        mMaxSamplesPerPixel = Octane.Renderer.GetLatestRenderStatistics().maxSamplesPerPixel;
    }

    IEnumerator WaitForOctaneRender()
    {
        string frame_str = "frame_" + Time.frameCount.ToString();
        while (!Octane.Renderer.GetLatestRenderStatistics().samplesPerPixel.Equals(mMaxSamplesPerPixel))
        {
            //print("Waiting for Octane to finish render ...");
            mCapturedFrame = false;
            Time.timeScale = 0.0f;
            yield return 0;
        }

        if (!mCapturedFrame)
        {
            Octane.Renderer.SaveImage(Octane.RenderPassId.RENDER_PASS_BEAUTY,
                                      mOutputDir + "/" + frame_str + "_" + Octane.Renderer.SampleCount + "spp",
                                      Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG8,
                                      false/*synchronous*/);
            Octane.Renderer.SaveImage(Octane.RenderPassId.RENDER_PASS_AMBIENT_OCCLUSION,
                                      mOutputDir + "/" + frame_str + "_AO_" + Octane.Renderer.SampleCount + "spp",
                                      Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG16,
                                      false/*synchronous*/);
            Octane.Renderer.SaveImage(Octane.RenderPassId.RENDER_PASS_Z_DEPTH,
                                      mOutputDir + "/" + frame_str + "_depth_" + Octane.Renderer.SampleCount + "spp",
                                      Octane.ImageSaveType.IMAGE_SAVE_TYPE_PNG16,
                                      false/*synchronous*/);
            print("Writing Octane image rendered with " + Octane.Renderer.GetLatestRenderStatistics().samplesPerPixel + "spp.");
            mCapturedFrame = true;
        }
        Time.timeScale = 1;
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(WaitForOctaneRender());
    }
}