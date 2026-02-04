export interface PortfolioSummary {
  name: string;
  role: string;
  yearsOfExperience: number;
  mainTechnologies: string[];
  heroBackgroundUrl: string;
}

export interface SocialLink {
  name: string;
  url: string;
}

export interface ContactInfo {
  email: string;
  phone: string;
  socialLinks: SocialLink[];
}

export interface ContactResponse {
  info: ContactInfo;
  cvEmailEnabled: boolean;
}

export interface ExperienceItem {
  position: string;
  company: string;
  from: string;
  to?: string | null;
  responsibilities: string[];
  periodText: string;
}

export interface AboutModel {
  bio: string;
  hobbies: string[];
  experience: ExperienceItem[];
}

export interface ProjectItem {
  title: string;
  description: string;
  company: string;
  technologies: string[];
}

export interface ChallengeItem {
  title: string;
  description: string;
}

export interface ProjectsModel {
  projects: ProjectItem[];
  challenges: ChallengeItem[];
}
