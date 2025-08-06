import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';
dayjs.extend(relativeTime);

export const formatMemberSince = createdAt =>
  `Member for ${dayjs(createdAt).fromNow(true)}`; // "16 years, 11 months"

export const formatLastSeen = lastVisit =>
  `Last seen ${dayjs(lastVisit).fromNow()}`;      // "this week" / "yesterday"