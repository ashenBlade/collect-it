import {RestrictionType} from "./restriction-type";

export default interface Restriction {
    readonly id: number;
    readonly restrictionType: RestrictionType;
    readonly authorId: number | null;
    readonly daysTo: number | null;
    readonly daysAfter: number | null;
    readonly tags: string[] | null;
    readonly size: number | null;
}