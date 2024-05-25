import * as React from 'react';
import type { IInsightChatProps } from './IInsightChatProps';
import { IInsightChatState } from './IInsightChatState';
import CompletionsService from '../services/CompletionsService';
import { ICompletionsResponse } from '../models/ICompletionsResponse';
import { Spinner, SpinnerSize, Stack } from "@fluentui/react";
import MessagesList from './MessagesList';
import UserMessage from './UserMessage';

export default class InsightChat extends React.Component<IInsightChatProps, IInsightChatState> {

  constructor(props: IInsightChatProps) {
    super(props);

    this.state = {
      userQuery: '',
      sessionMessages: [],
      workingOnIt: false
    };
  }

  private _onUserQueryChange = (newQuery: string): void => {
    this.setState({
      userQuery: newQuery
    });
  }

  private _onQuerySent = async (): Promise<void> => {
    console.log(this.state.userQuery);
    console.log(this.state.sessionMessages);

    this.setState({
      workingOnIt: true
    });

    const completionsService: CompletionsService = new CompletionsService(this.props.httpClient);

    const response: ICompletionsResponse =
      await completionsService.getCompletions(this.state.userQuery);

    console.log(response);

    const tempMessages = this.state.sessionMessages;
    tempMessages.push({
      role: 'user', text: this.state.userQuery
    });
    tempMessages.push({
      role: 'assistant', text: response.chatAnswer
    });

    this.setState({
      sessionMessages: tempMessages,
      userQuery: '',
      workingOnIt: false
    });
  }

  public render(): React.ReactElement<IInsightChatProps> {

    return (
      <Stack tokens={{ childrenGap: 20 }} style={{ minHeight: "100%" }}>
        <Stack.Item
          grow={1}
          styles={{
            root: { minHeight: "200px", height: "100%", position: "relative" },
          }}
        >
          <MessagesList messages={this.state.sessionMessages} />
        </Stack.Item>
        {this.state.workingOnIt && (
          <Stack.Item>
            <Spinner size={SpinnerSize.large} label="Wait till our super cool AI system is finding you an answer..." ariaLive="assertive" labelPosition="right" />
          </Stack.Item>
        )}
        <Stack.Item>
          <UserMessage
            textFieldValue={this.state.userQuery}
            onMessageChange={this._onUserQueryChange}
            sendQuery={this._onQuerySent}
          />
        </Stack.Item>
      </Stack>
    );
  }
}
