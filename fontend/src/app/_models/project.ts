export interface Project {
    projectId: string;
    code: string;
    name: string;
    description: string;
    startDate: string;
    endDate: string;
    modifiedDate: string;
    isDeleted: boolean;
    listLabel: ListLabel[];
    listUser: ListUser[];
}

export interface ListLabel {
    labelId: number;
    name: string;
    color: string;
}

export interface ListUser {
    userId: string;
    fullName: string;
    avatarUrl: string;
    role: string;
}