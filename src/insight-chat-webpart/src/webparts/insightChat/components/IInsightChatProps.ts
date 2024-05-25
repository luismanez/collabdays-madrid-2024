import {HttpClient} from '@microsoft/sp-http';

export interface IInsightChatProps {
  description: string;
  isDarkTheme: boolean;
  environmentMessage: string;
  hasTeamsContext: boolean;
  userDisplayName: string;
  httpClient: HttpClient;
}