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

export interface ActiveGameSet {
  /** @format uuid */
  playerId?: string;

  /** @format uuid */
  gameSetId?: string;
}

export interface Game {
  /** @format uuid */
  gameId?: string;
  connectedGameSets?: LasertagSet[] | null;
  activeGameSets?: ActiveGameSet[] | null;
  gameSetGroups?: GameGroup[] | null;
  status?: GameStatus;

  /** @format uuid */
  activeRoundId?: string;
}

export interface GameApiResult {
  output?: Game;
  success?: boolean;
  message?: string | null;
}

export interface GameGroup {
  /** @format uuid */
  groupId?: string;
  gameSets?: LasertagSet[] | null;
  color?: GroupColor;
}

export interface GameRound {
  /** @format uuid */
  id?: string;

  /** @format int32 */
  shotsFired?: number;
  playerStats?: PlayerStats[] | null;

  /** @format int32 */
  shotsHit?: number;

  /** @format int32 */
  version?: number;
  status?: GameRoundStatus;
  activeGameSets?: ActiveGameSet[] | null;
  gameSetGroups?: GameGroup[] | null;
}

export interface GameRoundApiResult {
  output?: GameRound;
  success?: boolean;
  message?: string | null;
}

export interface GameRoundStartResult {
  game?: GameApiResult;
  gameRound?: GameRoundApiResult;
}

export type GameRoundStatus = "Created" | "Started" | "Finished";

export type GameStatus =
  | "Initialized"
  | "LobyOpened"
  | "GameStarted"
  | "GameFinished"
  | "None";

export type GroupColor =
  | "Red"
  | "Blue"
  | "Yellow"
  | "Green"
  | "Turquoise"
  | "Pink"
  | "Violet"
  | "White";

export interface LasertagSet {
  /** @format uuid */
  id?: string;
  name?: string | null;
}

export interface PlayerStats {
  /** @format uuid */
  playerId?: string;

  /** @format uuid */
  gameSetId?: string;

  /** @format int32 */
  shotsFired?: number;

  /** @format int32 */
  gotHit?: number;

  /** @format int32 */
  shotsHit?: number;

  /** @format double */
  hitRatio?: number;
}
