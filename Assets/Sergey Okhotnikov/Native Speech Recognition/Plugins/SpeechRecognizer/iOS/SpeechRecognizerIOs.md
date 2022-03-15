# ``SpeechRecognizerIOs``

Native Speech Recognition framework

## Recognition start
Each recognition requires 3 arguments:

1. Speech language
2. Result callback
3. Error callback

typedef void (*StringDelegateCallback)(const char * string);

void start_speech_recognition(const char *language, StringDelegateCallback onResult, StringDelegateCallback onError);

onResult - returns full text beginig from recognition start but is called every time new word is recognized

## Recognition stop

void stop_speech_recognition()

## Supported languages
"nl-NL",
"es-MX",
"fr-FR",
"zh-TW",
"it-IT",
"vi-VN",
"fr-CH",
"es-CL",
"en-ZA",
"ko-KR",
"ca-ES",
"ro-RO",
"en-PH",
"es-419",
"en-CA",
"en-SG",
"en-IN",
"en-NZ",
"it-CH",
"fr-CA",
"hi-IN",
"da-DK",
"de-AT",
"pt-BR",
"yue-CN",
"zh-CN",
"sv-SE",
"hi-IN-translit",
"es-ES",
"ar-SA",
"hu-HU",
"fr-BE",
"en-GB",
"ja-JP",
"zh-HK",
"fi-FI",
"tr-TR",
"nb-NO",
"en-ID",
"en-SA",
"pl-PL",
"ms-MY",
"cs-CZ",
"el-GR",
"id-ID",
"hr-HR",
"en-AE",
"he-IL",
"ru-RU",
"wuu-CN",
"de-DE",
"de-CH",
"en-AU",
"nl-BE",
"th-TH",
"pt-PT",
"sk-SK",
"en-US",
"en-IE",
"es-CO",
"hi-Latn",
"uk-UA",
"es-US"


