import {ResourceType} from "./resource-type";
import Restriction from "./restriction";

export default interface Subscription {
    readonly id: number;
    readonly name: string;
    readonly description: string;
    readonly monthDuration: number;
    readonly appliedResourceType: ResourceType;
    readonly price: number;
    readonly restriction: Restriction | null;
    readonly active: boolean;
    readonly maxDownloadCount: number;
}