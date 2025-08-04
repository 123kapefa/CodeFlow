import React from "react";
import { Link } from 'react-router-dom';

import '../../App.css'

function Footer() {
  return (
    <div className="container-fluid footer-body">
      <footer className="footer container-xxl">
        <section>
          <h4>CodeFlow</h4>
          <ul>
            <li><a href="#">Questions</a></li>
            <li><a href="#">Help</a></li>
            <li><a href="#">Chat</a></li>
          </ul>
        </section>
        <section>
          <h4>PRODUCTS</h4>
          <ul>
            <li><a href="#">Teams</a></li>
            <li><a href="#">Advertising</a></li>
            <li><a href="#">Talent</a></li>
          </ul>
        </section>
        <section>
          <h4>COMPANY</h4>
          <ul>
            <li><a href="#">About</a></li>
            <li><a href="#">Press</a></li>
            <li><a href="#">Work Here</a></li>
            <li><a href="#">Legal</a></li>
          </ul>
        </section>
        <section>
          <h4>STACK EXCHANGE NETWORK</h4>
          <ul>
            <li><a href="#">Technology</a></li>
            <li><a href="#">Culture & recreation</a></li>
            <li><a href="#">Life & arts</a></li>
            <li><a href="#">Science</a></li>
          </ul>
        </section>
      </footer>
    </div>
  );
}

export default Footer;