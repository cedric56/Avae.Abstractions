export const textToSpeechInterop = {
    speak: function (text) {
        if (!window.speechSynthesis) {
            console.error("❌ Speech Synthesis API is not supported in this browser.");
            return;
        }

        let utterance = new SpeechSynthesisUtterance(text);
        window.speechSynthesis.speak(utterance);

        console.log("✅ Speaking:", text);
    },
    speakWithOptions: function (text, lang, voice, volume, pitch) {
        if (!window.speechSynthesis) {
            console.error("❌ Speech Synthesis API is not supported in this browser.");
            return;
        }

        this.resolveVoices().then(voices => {
            let utterance = new SpeechSynthesisUtterance(text);
            utterance.lang = lang;
            utterance.voice = voices.find(v => v.voiceURI == voice);
            utterance.volume = volume;
            utterance.pitch = pitch;
            window.speechSynthesis.speak(utterance);
            console.log("✅ Speaking:", text);
        });
    },
    getVoices: function () {
        return new Promise(async (resolve, reject) => {
            if (!window.speechSynthesis) {
                console.error("❌ Speech Synthesis API is not supported in this browser.");
                reject(null);
                return;
            }
            this.resolveVoices().then(voices => {
                resolve(JSON.stringify(voices.map(voice => ({
                    name: voice.name,
                    lang: voice.lang,
                    default: voice.default,
                    voiceURI: voice.voiceURI,
                    localService: voice.localService
                })), null, 2));
            });
        });
    },
    resolveVoices: function () {
        return new Promise((resolve) => {
            let voices = window.speechSynthesis.getVoices();
            if (voices.length !== 0) {
                resolve(voices);
            }
            else {
                window.speechSynthesis.addEventListener("voiceschanged", function () {
                    voices = window.speechSynthesis.getVoices();
                    resolve(voices);
                });
            }
        });
    }
};