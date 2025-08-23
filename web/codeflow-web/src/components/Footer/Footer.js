import React from "react";

import '../../App.css'

function Footer() {
  return (
    <div className="container-fluid footer-body">
      <footer className="footer">
        <section>
          <h4>CodeFlow</h4>
          <ul>
            <li><a href="/">Questions</a></li>
            <li><a href="/help">Help</a></li>          
          </ul>
        </section>
        <section>
          <h4>PRODUCTS</h4>
          <ul>
            <li><a href="/teams">Teams</a></li>
            <li><a href="/advertising">Advertising</a></li>
            <li><a href="/talent">Talent</a></li>
          </ul>
        </section>
        <section>
          <h4>COMPANY</h4>
          <ul>
            <li><a href="/about">About</a></li>
            <li><a href="/press">Press</a></li>
            <li><a href="/work-here">Work Here</a></li>
            <li><a href="/legal">Legal</a></li>
          </ul>
        </section>
        <section>
          <h4>STACK EXCHANGE NETWORK</h4>
          <ul>
            <li><a href="/technology">Technology</a></li>
            <li><a href="/culture">Culture & recreation</a></li>
            <li><a href="/life">Life & arts</a></li>
            <li><a href="/science">Science</a></li>
          </ul>
        </section>
      </footer>
    </div>
  );
}

export default Footer;