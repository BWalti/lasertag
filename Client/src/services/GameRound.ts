/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

import { GameRoundApiResult } from "./data-contracts";
import { HttpClient, RequestParams } from "./http-client";

export class GameRound<SecurityDataType = unknown> {
  http: HttpClient<SecurityDataType>;

  constructor(http: HttpClient<SecurityDataType>) {
    this.http = http;
  }

  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name LasertagSetShotFiredCreate
   * @request POST:/gameRound/{gameRoundId}/lasertagSet/{sourceLasertagSetId}/shotFired
   */
  lasertagSetShotFiredCreate = (
    gameRoundId: string,
    sourceLasertagSetId: string,
    params: RequestParams = {},
  ) =>
    this.http.request<GameRoundApiResult, any>({
      path: `/gameRound/${gameRoundId}/lasertagSet/${sourceLasertagSetId}/shotFired`,
      method: "POST",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name LasertagSetGotHitFromLasertagSetCreate
   * @request POST:/gameRound/{gameRoundId}/lasertagSet/{sourceLasertagSetId}/got-hit-from-lasertagSet/{targetLasertagSetId}
   */
  lasertagSetGotHitFromLasertagSetCreate = (
    gameRoundId: string,
    sourceLasertagSetId: string,
    targetLasertagSetId: string,
    params: RequestParams = {},
  ) =>
    this.http.request<GameRoundApiResult, any>({
      path: `/gameRound/${gameRoundId}/lasertagSet/${sourceLasertagSetId}/got-hit-from-lasertagSet/${targetLasertagSetId}`,
      method: "POST",
      format: "json",
      ...params,
    });
}
