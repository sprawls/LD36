/*
 * Copyright (c) 2015 Allan Pichardo
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;
using System.Collections;


/*
 * Make your class implement the interface AudioProcessor.AudioCallbaks
 */
public class BeatDetect : MonoBehaviour, AudioProcessor.AudioCallbacks
{
    AudioProcessor processor;
    Vector3 beatScale;
    public float dropTime = 0.2f;

    Vector3 initialScale;
    
    void Start()
    {
        //Select the instance of AudioProcessor and pass a reference
        //to this object
        processor = FindObjectOfType<AudioProcessor>();
        processor.addAudioCallback(this);

        initialScale = transform.localScale;
        float scale = Random.Range(0.01f, 0.04f);
        beatScale = new Vector3(scale, scale, scale);
    }

    
    void Update()
    {
        
    }

    //this event will be called every time a beat is detected.
    //Change the threshold parameter in the inspector
    //to adjust the sensitivity
    public void onOnbeatDetected()
    {
        transform.localScale = initialScale + beatScale;
        StopCoroutine("beatScaleDrop");
        StartCoroutine("beatScaleDrop");
    }

    //This event will be called every frame while music is playing
    public void onSpectrum(float[] spectrum)
    {
        //The spectrum is logarithmically averaged
        //to 12 bands
       
        for (int i = 0; i < spectrum.Length; ++i)
        {
            Vector3 start = new Vector3(i, 0, 0);
            Vector3 end = new Vector3(i, spectrum[i], 0);
            Debug.DrawLine(start, end);
        }
    }

    IEnumerator beatScaleDrop()
    {
        for (float i = 0; i < 1.0f; i += Time.deltaTime / dropTime)
        {
            transform.localScale -= beatScale * Time.deltaTime / dropTime;
            yield return null;
        }
    }

    void OnDestroy()
    {
        processor.removeAudioCallback(this);
    }
}
