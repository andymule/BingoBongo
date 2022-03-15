//
//  SpeechRecognizer.h
//  SpeechRecognizerIOs
//
//  Created by Sergey Okhotnikov on 12/01/2022.
//

NS_ASSUME_NONNULL_BEGIN


typedef void (*StringDelegateCallback)(const char * string);

@interface TaskHolder : NSObject

@property StringDelegateCallback onResult;
@property StringDelegateCallback onError;
@property NSString * language;

- (void) result:(const char *) value;
- (void) error:(const char *) value;

@end
NS_ASSUME_NONNULL_END
