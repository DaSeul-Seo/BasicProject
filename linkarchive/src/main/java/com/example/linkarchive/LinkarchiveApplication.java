package com.example.linkarchive;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.data.jpa.repository.config.EnableJpaAuditing;

@SpringBootApplication
@EnableJpaAuditing	// @CreatedDate 작동을 위해
public class LinkarchiveApplication {

	public static void main(String[] args) {
		SpringApplication.run(LinkarchiveApplication.class, args);
	}

}
