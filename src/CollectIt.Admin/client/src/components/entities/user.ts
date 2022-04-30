import Subscription from "./subscription";
import Resource from "./resource";

export default interface User {
    readonly id: number;
    readonly username: string;
    readonly email: string;
    readonly roles: string[];
    readonly subscriptions: Subscription[];
    readonly authorOf: Resource[];
    readonly lockout: boolean;
    readonly lockoutEnd: Date;
}