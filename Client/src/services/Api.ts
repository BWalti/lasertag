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
   * @name InitGame
   * @request POST:/api/game/init
   */
  initGame = (params: RequestParams = {}) =>
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
   * @name RegisterGameSet
   * @request POST:/api/game/{gameId}/{gameSetId}
   */
  registerGameSet = (
    gameId: string,
    gameSetId: string,
    query: { isTargetOnly: boolean },
    params: RequestParams = {},
  ) =>
    this.http.request<GameApiResult, any>({
      path: `/api/game/${gameId}/${gameSetId}`,
      method: "POST",
      query: query,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name ConnectGameSet
   * @request POST:/api/game/{gameId}/{gameSetId}/connect
   */
  connectGameSet = (
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
   * @name DisconnectGameSet
   * @request POST:/api/game/{gameId}/{gameSetId}/disconnect
   */
  disconnectGameSet = (
    gameId: string,
    gameSetId: string,
    params: RequestParams = {},
  ) =>
    this.http.request<GameApiResult, any>({
      path: `/api/game/${gameId}/${gameSetId}/disconnect`,
      method: "POST",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name CreateGameRound
   * @request POST:/api/game/{gameId}/createLobby
   */
  createGameRound = (
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
   * @name StartGameRound
   * @request POST:/api/game/{gameId}/start
   */
  startGameRound = (gameId: string, params: RequestParams = {}) =>
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
   * @name GameRoundLasertagSetActivateCreate
   * @request POST:/api/gameRound/{gameRoundId}/lasertagSet/{gameSetId}/activate/{playerId}
   */
  gameRoundLasertagSetActivateCreate = (
    gameRoundId: string,
    gameSetId: string,
    playerId: string,
    params: RequestParams = {},
  ) =>
    this.http.request<GameRoundApiResult, any>({
      path: `/api/gameRound/${gameRoundId}/lasertagSet/${gameSetId}/activate/${playerId}`,
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
