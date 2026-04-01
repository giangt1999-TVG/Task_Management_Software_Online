import { Role } from "./role";

export interface User {
    id: string;
    email: string;
    userName: string;
    fullName: string;
    avatarUrl: string;
    mainRole: Role;
    token: string;
    description: string;
}