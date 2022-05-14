export interface HubMessage {
    username: string;
    sendedDateUtc: Date;
    message: string;
    group: string;
}