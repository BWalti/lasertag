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

import {
  GameApiResult,
  GameRoundApiResult,
  GameRoundStartResult,
  ScoreBoardApiResult,
} from "./data-contracts";
import { HttpClient, RequestParams } from "./http-client";

export class Api<SecurityDataType = unknown> {
  http: HttpClient<SecurityDataType>;

  constructor(http: HttpClient<SecurityDataType>) {
    this.http = http;
  }

  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name GameInitCreate
   * @request POST:/api/game/init
   */
  gameInitCreate = (params: RequestParams = {}) =>
    this.http.request<GameApiResult, any>({
      path: `/api/game/init`,
      method: "POST",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name GameConnectCreate
   * @request POST:/api/game/{gameId}/{gameSetId}/connect
   */
  gameConnectCreate = (
    gameId: string,
    gameSetId: string,
    params: RequestParams = {},
  ) =>
    this.http.request<GameApiResult, any>({
      path: `/api/game/${gameId}/${gameSetId}/connect`,
      method: "POST",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name GameCreateLobbyCreate
   * @request POST:/api/game/{gameId}/createLobby
   */
  gameCreateLobbyCreate = (
    gameId: string,
    query?: { numberOfGroups?: number },
    params: RequestParams = {},
  ) =>
    this.http.request<GameApiResult, any>({
      path: `/api/game/${gameId}/createLobby`,
      method: "POST",
      query: query,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name GameActivateCreate
   * @request POST:/api/game/{gameId}/{gameSetId}/activate/{playerId}
   */
  gameActivateCreate = (
    gameId: string,
    gameSetId: string,
    playerId: string,
    params: RequestParams = {},
  ) =>
    this.http.request<GameApiResult, any>({
      path: `/api/game/${gameId}/${gameSetId}/activate/${playerId}`,
      method: "POST",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name GameStartCreate
   * @request POST:/api/game/{gameId}/start
   */
  gameStartCreate = (gameId: string, params: RequestParams = {}) =>
    this.http.request<GameRoundStartResult, any>({
      path: `/api/game/${gameId}/start`,
      method: "POST",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name GameRoundLasertagSetShotFiredCreate
   * @request POST:/api/gameRound/{gameRoundId}/lasertagSet/{sourceLasertagSetId}/shotFired
   */
  gameRoundLasertagSetShotFiredCreate = (
    gameRoundId: string,
    sourceLasertagSetId: string,
    params: RequestParams = {},
  ) =>
    this.http.request<GameRoundApiResult, any>({
      path: `/api/gameRound/${gameRoundId}/lasertagSet/${sourceLasertagSetId}/shotFired`,
      method: "POST",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name GameRoundLasertagSetGotHitFromLasertagSetCreate
   * @request POST:/api/gameRound/{gameRoundId}/lasertagSet/{sourceLasertagSetId}/got-hit-from-lasertagSet/{targetLasertagSetId}
   */
  gameRoundLasertagSetGotHitFromLasertagSetCreate = (
    gameRoundId: string,
    sourceLasertagSetId: string,
    targetLasertagSetId: string,
    params: RequestParams = {},
  ) =>
    this.http.request<GameRoundApiResult, any>({
      path: `/api/gameRound/${gameRoundId}/lasertagSet/${sourceLasertagSetId}/got-hit-from-lasertagSet/${targetLasertagSetId}`,
      method: "POST",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name GameRoundStatsDetail
   * @request GET:/api/gameRound/{gameRoundId}/stats
   */
  gameRoundStatsDetail = (gameRoundId: string, params: RequestParams = {}) =>
    this.http.request<ScoreBoardApiResult, any>({
      path: `/api/gameRound/${gameRoundId}/stats`,
      method: "GET",
      format: "json",
      ...params,
    });
}
