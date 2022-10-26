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
  gameSets?: GameSet[] | null;
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
  gameSets?: GameSet[] | null;
  color?: GroupColor;
}

export interface GameRound {
  /** @format uuid */
  id?: string;
  gameSetGroups?: GameGroup[] | null;

  /** @format int32 */
  version?: number;
  status?: GameRoundStatus;
  activeGameSets?: ActiveGameSet[] | null;
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

export interface GameSet {
  /** @format uuid */
  id?: string;
  isOnline?: boolean;
  configuration?: GameSetConfiguration;
  name?: string | null;
}

export interface GameSetConfiguration {
  /** @format uuid */
  id?: string;
  isTargetOnly?: boolean;
}

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

export interface ScoreBoard {
  /** @format uuid */
  id?: string;

  /** @format int32 */
  shotsFired?: number;

  /** @format int32 */
  shotsHit?: number;
}

export interface ScoreBoardApiResult {
  output?: ScoreBoard;
  success?: boolean;
  message?: string | null;
}
