//
//
// Copyright (c) 2017 Fyber. All rights reserved.
//
//

@import Foundation;

@interface FYBCredentials : NSObject<NSCopying>

@property (nonatomic, copy) NSString *appId;
@property (nonatomic, copy) NSString *userId;
@property (nonatomic, copy) NSString *securityToken;

+ (FYBCredentials *)credentialsWithAppId:(NSString *)appId userId:(NSString *)userId securityToken:(NSString *)securityToken;

@end
