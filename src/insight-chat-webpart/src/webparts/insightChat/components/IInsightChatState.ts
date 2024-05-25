import { IChatMessage } from "../models/IChatMessage";

export interface IInsightChatState {
  userQuery: string;
  sessionMessages: IChatMessage[];
  workingOnIt: boolean;
}
