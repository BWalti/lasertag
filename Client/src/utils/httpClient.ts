import { baseAddress } from "./env";
import { HttpClient, ApiConfig } from "../services/http-client";

const config: ApiConfig = {
  baseUrl: baseAddress,
};

export const lasertagApiHttpClient = new HttpClient(config);
