export interface Notifications {
    notificationId?: number;
    content?: string | Content;
    isViewed?: boolean;
    createdDate?: Date;
}
export interface Content {
    Title?: string;
    Content?: string;
}
