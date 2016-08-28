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
public abstract class BeatDetect : MonoBehaviour, AudioProcessor.AudioCallbacks
{
    AudioProcessor processor;
    
    // Initialize BeatDetect after AudioProcessor is initialized
    protected virtual void Awake() {
        StartCoroutine(InitializeBeatDetector());
    }

    IEnumerator InitializeBeatDetector()
    {
        while (true)
        {
            if (AudioProcessor.initialized == false) yield return new WaitForSeconds(0.5f);
            else
            {
                SetupBeat();
                break;
            }
        }
    }
    
    // link gameObject to audio processor
    protected void SetupBeat() {
        processor = FindObjectOfType<AudioProcessor>();
        processor.addAudioCallback(this);
    }

    //this event will be called every time a beat is detected
    public abstract void onOnbeatDetected();

    //This event will be called every frame while music is playing
    public abstract void onSpectrum(float[] spectrum);

    // Unlink gameObject to audio processor
    void OnDestroy()
    {
        if(processor != null) processor.removeAudioCallback(this);
    }

   
}
// this shows lines according to music beat
/*for (int i = 0; i < spectrum.Length; ++i)
    {
        Vector3 start = new Vector3(i, 0, 0);
        Vector3 end = new Vector3(i, spectrum[i], 0);
        Debug.DrawLine(start, end);
    }*/
