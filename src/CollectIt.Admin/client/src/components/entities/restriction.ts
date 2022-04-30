import {RestrictionType} from "./restriction-type";

export default interface Restriction {
    readonly id: number;
    readonly restrictionType: RestrictionType;
    readonly authorId?: number;
    readonly daysTo?: number;
    readonly daysAfter?: number;
    readonly tags?: string[];
    readonly size?: number;
}