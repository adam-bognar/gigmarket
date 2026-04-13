import { ChangeDetectionStrategy, Component } from '@angular/core';
import {Hero} from './components/hero/hero';
import {CategoryStrip} from './components/category-strip/category-strip';
import {FeaturedGigs} from './components/featured-gigs/featured-gigs';
import {CtaBanner} from './components/cta-banner/cta-banner';
import {Features} from './components/features/features';

@Component({
  selector: 'app-landing',
  imports: [Hero, CategoryStrip, Features, FeaturedGigs, CtaBanner],
  templateUrl: './landing.html',
  styleUrl: './landing.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Landing {
  //TODO use proper categories
  readonly footerLinks = [
    {
      title: 'Categories',
      links: ['Graphics & Design', 'Digital Marketing', 'Writing & Translation', 'Video & Animation', 'Music & Audio'],
    },
    {
      title: 'About',
      links: ['Careers', 'Press & News', 'Partnerships', 'Privacy Policy', 'Terms of Service'],
    },
    {
      title: 'Support',
      links: ['Help & Support', 'Trust & Safety', 'Selling on GigMarket', 'Buying on GigMarket'],
    },
  ];

  readonly legalLinks = ['Privacy Policy', 'Terms of Service', 'Cookie Settings'];
}
