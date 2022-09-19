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

import { GameApiResult, GameRoundStartResult } from "./data-contracts";
import { HttpClient, RequestParams } from "./http-client";

export class Game<SecurityDataType = unknown> {
  http: HttpClient<SecurityDataType>;

  constructor(http: HttpClient<SecurityDataType>) {
    this.http = http;
  }

  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name InitCreate
   * @request POST:/game/init
   */
  initCreate = (params: RequestParams = {}) =>
    this.http.request<GameApiResult, any>({
      path: `/game/init`,
      method: "POST",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name ConnectCreate
   * @request POST:/game/{gameId}/{gameSetId}/connect
   */
  connectCreate = (
    gameId: string,
    gameSetId: string,
    params: RequestParams = {},
  ) =>
    this.http.request<GameApiResult, any>({
      path: `/game/${gameId}/${gameSetId}/connect`,
      method: "POST",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name CreateLobbyCreate
   * @request POST:/game/{gameId}/createLobby
   */
  createLobbyCreate = (
    gameId: string,
    query?: { numberOfGroups?: number },
    params: RequestParams = {},
  ) =>
    this.http.request<GameApiResult, any>({
      path: `/game/${gameId}/createLobby`,
      method: "POST",
      query: query,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name ActivateCreate
   * @request POST:/game/{gameId}/{gameSetId}/activate/{playerId}
   */
  activateCreate = (
    gameId: string,
    gameSetId: string,
    playerId: string,
    params: RequestParams = {},
  ) =>
    this.http.request<GameApiResult, any>({
      path: `/game/${gameId}/${gameSetId}/activate/${playerId}`,
      method: "POST",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Admin.Api, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
   * @name StartCreate
   * @request POST:/game/{gameId}/start
   */
  startCreate = (gameId: string, params: RequestParams = {}) =>
    this.http.request<GameRoundStartResult, any>({
      path: `/game/${gameId}/start`,
      method: "POST",
      format: "json",
      ...params,
    });
}
